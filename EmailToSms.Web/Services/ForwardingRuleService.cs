using EmailToSms.Web.ViewModels;

namespace EmailToSms.Web.Services
{
    //kullanıcının arayüzde yaptığı kural işlemlerini paketleyip ağ üzerinden api tarafındaki kontrollera gönderir
    public class ForwardingRuleService
    {
        private readonly HttpClient _httpClient;
        public ForwardingRuleService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        //dışarıdan e-posta Id si alır ve ona bağlı kuralları getirir
        public async Task<List<ForwardingRuleViewModel>> GetRulesByAccountIdAsync(int emailAccountId)
        {
            var response = await _httpClient.GetAsync($"api/ForwardingRules/email/{emailAccountId}");
            if (response.IsSuccessStatusCode)
            {
                //dışarıdan alınan JSON formatındaki veriyi Web katmanımızın anlayacağı ViewModel formatına çevirir
                return await response.Content.ReadFromJsonAsync<List<ForwardingRuleViewModel>>() ?? new List<ForwardingRuleViewModel>();
            }
            return new List<ForwardingRuleViewModel>();
        }

        //formdan gelen kural verilerini apiye gönderir
        public async Task<bool> AddRuleAsync(ForwardingRuleCreateViewModel model)
        {
            //json formatına çevirip yollar
            var response = await _httpClient.PostAsJsonAsync("api/ForwardingRules" , model);
            return response.IsSuccessStatusCode;
        }

        //formdan gelen verileri id ye göre apiye gönderir
        public async Task<bool> UpdateRuleAsync(int id , ForwardingRuleUpdateViewModel model)
        {
            //json formatına çevirip gönderir
            var response = await _httpClient.PutAsJsonAsync($"api/ForwardingRules/{id}", model);
            return response.IsSuccessStatusCode;
        }

        //silinecek kuralı apiye gönderir
        public async Task<bool> DeleteRuleAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ForwardingRules/{id}");

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
