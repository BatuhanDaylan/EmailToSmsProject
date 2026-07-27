using Microsoft.AspNetCore.RateLimiting;
using EmailToSms.Api.Data;
using EmailToSms.Api.Middlewares;
using EmailToSms.Api.Models;
using EmailToSms.Api.Services;
using EmailToSms.Api.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:5002" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", p =>
    {
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    // "LoginPolicy" adında bir kural tanımlıyoruz
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1); // 1 dakikalık zaman dilimi
        opt.PermitLimit = 5; // Maksimum 5 istek 
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // Sınır aşılırsa kuyruğa almadan direkt HTTP 429  döndür
    });

    // Sınır aşıldığında verilecek yanıtı özelleştirebilirsin
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync("{\"isSuccess\":false, \"message\":\"Çok fazla giriş denemesi yaptınız. Lütfen 1 dakika bekleyip tekrar deneyin.\"}");
    };
});

builder.Services.AddDataProtection();

builder.Services.AddDbContext<EmailToSmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<EmailToSmsIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddIdentity<EmailToSmsUser, IdentityRole>()
    .AddEntityFrameworkStores<EmailToSmsIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<IMicrosoftGraphEmailReader, MicrosoftGraphEmailReader>();

builder.Services.AddHostedService<EmailCheckerWorker>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailToSms API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Lütfen 'Bearer {token}' formatında giriş yapın. Örnek: Bearer eyJhbG...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();