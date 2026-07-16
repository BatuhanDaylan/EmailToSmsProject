using EmailToSms.Web.ViewModels;

namespace EmailToSms.Web.Services
{
    public class EmailAccountService
    {
        private readonly HttpClient _httpClient;

        public EmailAccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        //apiden veri çekme işlemi
        public async Task<List<EmailAccountViewModel>> GetAccountsAsync()
        {
            var response = await _httpClient.GetAsync("api/EmailAccounts");

            //eğer başarısız olursa hata vermez boş liste döner
            if (response.IsSuccessStatusCode) 
            {
                return await response.Content.ReadFromJsonAsync<List<EmailAccountViewModel>>() ?? new List<EmailAccountViewModel>();
            }
            return new List<EmailAccountViewModel>();
        }

    }
}
