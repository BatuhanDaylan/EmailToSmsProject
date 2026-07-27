using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace EmailToSms.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IHttpClientFactory httpClientFactory , IHttpContextAccessor httpContextAccessor)
        {
            //program.cs te tanımladığımız ApiClient i çağırıyoruz
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(object loginViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginViewModel);
            if (response.IsSuccessStatusCode)
            {
                // DÜZELTME: API'den gelen veriyi artık ApiResponse zarfıyla (Result Pattern) karşılıyoruz
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

                // Zarfın içindeki Data doluysa ve Token boş değilse
                if (apiResponse?.Data != null && !string.IsNullOrEmpty(apiResponse.Data.Token))
                {
                    // token handlerın kullanabilmesi için tokenı sessiona kaydediyoruz
                    _httpContextAccessor.HttpContext?.Session.SetString("ApiJwt", apiResponse.Data.Token);
                    return true;
                }
            }
            return false;
        }

        //Sadece bool değil, hata mesajını da taşıyan bir Tuple dönüyoruz
        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(object registerViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerViewModel);

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }

            //API işlemi reddettiyse, gönderdiği asıl hata mesajını okumaya çalışıyoruz
            try
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
                string errorMessage = apiResponse?.Message ?? "Bilinmeyen bir hata oluştu.";

                // Eğer API'nin ErrorDetail alanı doluysa (Identity hataları genelde oraya düşer)
                if (!string.IsNullOrEmpty(apiResponse?.ErrorDetails))
                {
                    errorMessage = apiResponse.ErrorDetails;
                }

                return (false, errorMessage);
            }
            catch
            {
                //Eğer Result Pattern parse edilemezse varsayılan bir uyarı ver
                return (false, "Kayıt reddedildi. Şifrenizin büyük harf, küçük harf, rakam ve özel karakter (*, !, ?) içerdiğinden emin olun.");
            }
        }

        public void Logout()
        {
            //çıkış yapıldığında sessiondaki tokenı siliyoruz
            _httpContextAccessor.HttpContext?.Session.Remove("ApiJwt");
        }

        public class AuthResponse
        {
            public string Token { get; set; } = string.Empty;
            public string Expiration { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
        }
    }
}
