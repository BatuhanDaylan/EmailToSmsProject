using EmailToSms.Web.ViewModels;
using Microsoft.Graph.Models;

namespace EmailToSms.Web.Services
{
    public class EmailAccountService
    {
        private readonly HttpClient _httpClient;

        public EmailAccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        //apiden veri çekme listeleme işlemi
        public async Task<List<EmailAccountViewModel>> GetAccountsAsync()
        {
            var response = await _httpClient.GetAsync("api/EmailAccounts");

            // eğer başarısız olursa hata vermez boş liste döner
            if (response.IsSuccessStatusCode)
            {
                // DÜZELTME: Veriyi ApiResponse zarfıyla karşılıyoruz
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EmailAccountViewModel>>>();
                return apiResponse?.Data ?? new List<EmailAccountViewModel>();
            }
            return new List<EmailAccountViewModel>();
        }

        //girilen veriyi apiye gönderme
        public async Task<bool> AddAccountAsync (EmailAccountCreateViewModel model)
        {
            //veri girişi
            var apiPayload = new
            {
                EmailAddress = model.EmailAddress,
                TenantId = model.TenantId,
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret
            };

            //eklenen verileri apiye gönderme
            var response = await _httpClient.PostAsJsonAsync("api/EmailAccounts", apiPayload);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Reddetme Detayı: {error}");
            }

            return true;
        }

        //veri düzenleyip apiye gönderme
        public async Task<bool> UpdateAccountAsync(int id , EmailAccountUpdateViewModel model)
        {
            //veri girişi
            var apiPayload = new
            {
                EmailAddress = model.EmailAddress,
                TenantId = model.TenantId,
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret
            };
            //eklenen verileri apiye gönderme
            var response = await _httpClient.PutAsJsonAsync($"api/EmailAccounts/{id}", apiPayload);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Reddetme Detayı: {error}");
            }
            return true;
        }

        public async Task<bool> ToggleAccountStatusAsync(int id)
        {
            var response = await _httpClient.PatchAsync($"api/EmailAccounts/{id}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }

        //veri silme
        public async Task<bool> DeleteAccountAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/EmailAccounts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<DashboardStatsViewModel> GetStatsAsync()
        {
            var response = await _httpClient.GetAsync("api/Stats");
            if (!response.IsSuccessStatusCode)
            {
                return new DashboardStatsViewModel(); // 0'larla dön
            }

            // DÜZELTME: Veriyi ApiResponse zarfıyla karşılıyoruz
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsViewModel>>();
            return apiResponse?.Data ?? new DashboardStatsViewModel();
        }
    }
}
