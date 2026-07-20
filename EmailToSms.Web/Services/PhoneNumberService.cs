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

        //dışarıdan e-posta Id si alır ve ona bağlı kuralları getirir
        public async Task<List<PhoneNumberViewModel>> GetNumberByRuleAsync(int ruleId)
        {
            var response = await _httpClient.GetAsync($"api/PhoneNumbers/rule/{ruleId}");
            if(response.IsSuccessStatusCode)
            {
                //dışarıdan alınan JSON formatındaki veriyi Web katmanımızın anlayacağı ViewModel formatına çevirir
                return await response.Content.ReadFromJsonAsync<List<PhoneNumberViewModel>>() ?? new List<PhoneNumberViewModel>();
            }
            return new List<PhoneNumberViewModel>();
        }

        //formdan gelen kural verilerini apiye gönderir
        public async Task<bool> AddPhoneNumberAsync(PhoneNumberCreateViewModel model)
        {
            //json formatına çağırıp gönderir
            var response = await _httpClient.PostAsJsonAsync("api/PhoneNumbers", model);
            return response.IsSuccessStatusCode;
        }

        //formdan gelen verileri idye göre apiye gönderir
        public async Task<bool> UpdatePhoneNumberAsync(int id , PhoneNumberUpdateViewModel model)
        {
            //json formatına çağırıp gönderir
            var response = await _httpClient.PutAsJsonAsync($"api/PhoneNumbers/{id}", model);
            return response.IsSuccessStatusCode;
        }

        //silinecek kuralı apiye yolla
        public async Task<bool > DeletePhoneNumberAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/PhoneNumbers{id}");

            //Eğer API işlemi onaylamazsa 
            if (!response.IsSuccessStatusCode)
            {
                //API'nin gönderdiği arka plan hata detayını okuyoruz
                var errorContent = await response.Content.ReadAsStringAsync();

                //Hatayı Controller'a fırlatıyoruz ki ekrandaki kırmızı kutuya yazsın
                throw new Exception($"Kod: {response.StatusCode} | Detay: {errorContent}");
            }

            return true;

        }
    }
}
