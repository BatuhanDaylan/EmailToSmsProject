using EmailToSms.Web.ViewModels;

namespace EmailToSms.Web.Services
{
    public class PhoneNumberService
    {
        private readonly HttpClient _httpClient;

        public PhoneNumberService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        //kullanıcının tüm rehberinin gelmesi
        public async Task<List<PhoneNumberViewModel>> GetMyPhoneBookAsync()
        {
            var response = await _httpClient.GetAsync("api/PhoneNumbers");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PhoneNumberViewModel>>>();
                return result?.Data ?? new List<PhoneNumberViewModel>();
            }
            return new List<PhoneNumberViewModel>();
        }

        //sadece belirli bir kurala bağlı numaraların getirilesi
        public async Task<List<PhoneNumberViewModel>> GetNumbersByRuleAsync(int ruleId)
        {
            var response = await _httpClient.GetAsync($"api/PhoneNumbers/rule/{ruleId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PhoneNumberViewModel>>>();
                return result?.Data ?? new List<PhoneNumberViewModel>();
            }
            return new List<PhoneNumberViewModel>();
        }

        //rehbere yeni numara ekle
        public async Task<bool> AddPhoneNumberAsync(PhoneNumberCreateViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PhoneNumbers", model);
            return response.IsSuccessStatusCode;
        }

        //rehberdeki numarayı güncelle
        public async Task<bool> UpdatePhoneNumberAsync(int id, PhoneNumberUpdateViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/PhoneNumbers/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Güncelleme Başarısız: {error}");
            }
            return true;
        }

        //numarayı rehberden tamamen sil
        public async Task<bool> DeletePhoneNumberAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/PhoneNumbers/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Silme Başarısız: {errorContent}");
            }
            return true;
        }

        //numarayı kurala bağla
        public async Task<bool> AssignToRuleAsync(PhoneRuleAssignViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PhoneNumbers/assign", model);
            return response.IsSuccessStatusCode;
        }

        //numarayı kuraldan kopar
        public async Task<bool> UnassignFromRuleAsync(PhoneRuleAssignViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PhoneNumbers/unassign", model);
            return response.IsSuccessStatusCode;
        }
    }
}