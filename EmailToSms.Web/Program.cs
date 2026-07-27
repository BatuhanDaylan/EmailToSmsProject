using EmailToSms.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session ve HttpContext erişimi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // prod için Always yap
    options.IdleTimeout = TimeSpan.FromHours(1);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenHandler>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailAccountService>();
builder.Services.AddScoped<ForwardingRuleService>();
builder.Services.AddScoped<PhoneNumberService>();

builder.Services.AddHttpClient<AuditLogService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<TokenHandler>(); //EKSİK OLAN VE 401 HATASINI ÇÖZEN SATIR

// Named HttpClient; API base url appsettings.json içinden okunur
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<TokenHandler>();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Sadece senin bilgisayarında (geliştirme ortamında) detaylı hata göstersin
    app.UseDeveloperExceptionPage();
}
else
{
    //Canlı ortamda (Production) son kullanıcıya standart hata sayfası göster
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();