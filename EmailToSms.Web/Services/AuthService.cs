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
                // REFACTORING: API'den gelen verinin null olma durumlarına karşı güvenli okuma
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

                if (apiResponse != null)
                {
                    string errorMessage = !string.IsNullOrEmpty(apiResponse.ErrorDetails)
                        ? apiResponse.ErrorDetails
                        : (apiResponse.Message ?? "Bilinmeyen bir hata oluştu.");

                    return (false, errorMessage);
                }

                return (false, "Sunucudan boş yanıt alındı.");
            }
            catch (System.Text.Json.JsonException) // REFACTORING: Sadece JSON parse hatalarını yakala
            {
                // API JSON yerine düz metin veya HTML dönerse patlamaması için
                return (false, "Sunucu ile iletişimde format hatası oluştu. Lütfen tekrar deneyin.");
            }
            catch (Exception ex)
            {
                return (false, $"Beklenmeyen bir hata oluştu: {ex.Message}");
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
