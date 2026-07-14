using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;


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
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login" , loginViewModel);
            if(response.IsSuccessStatusCode)
            {
                //apiden dönen token bilgisini okuyoruz
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if(result != null && !string.IsNullOrEmpty(result.Token))
                {
                    //token handlerın kullanabilmesi için tokenı sessiona kaydediyoruz
                    _httpContextAccessor.HttpContext?.Session.SetString("ApiJwt", result.Token);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RegisterAsync(object registerViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register" , registerViewModel);
            return response.IsSuccessStatusCode;
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
