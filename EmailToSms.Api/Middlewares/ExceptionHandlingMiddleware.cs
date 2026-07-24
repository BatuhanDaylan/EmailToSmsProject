using EmailToSms.Api.Common;
using System.Net;
using System.Text.Json;

namespace EmailToSms.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next , ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //hatayı loglama
            _logger.LogError(exception , "Sistemde bir hata oluştu.");

            //response başlıklarını ayarlıyoruz
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //hatayı ortak result sınıfı ile gönderiyoruz
            var response = Result.Failure("Sunucu tarafında beklenmedik bir hata oluştu.", exception.Message);

            //result nesnemizi json formatına çevirip önyüzümüze gönderiyoruz
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        //hatayönetimi yapacak middleware sınıfı buraya yazılacak
        //sonraki adıma geçmek için RequestDelegate hatayı loglamak için ILogger lazım
        //dependency injection yapmalıyız
        //gelen tüm istekleri alacak bir method
        //try catch blokları kullan try içinde isteği serbest bırak catch kısmında hatayı yakala
        //hatayı işleyip stabilize edecek bir method uygun formatta geri döndürmeli
    }
}
