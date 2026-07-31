using EmailToSms.Api.Services;
using System.Text;
using System.Text.Json;

namespace EmailToSms.Api.Services
{
    public class CompanySmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CompanySmsService> _logger;

        public CompanySmsService(HttpClient httpClient, IConfiguration configuration, ILogger<CompanySmsService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            string formattedPhone = "0" + phoneNumber.Substring(3);

            var apiUrl = _configuration["SmsSettings:ApiUrl"];
            var apiKey = _configuration["SmsSettings:ApiKey"];

            //Payload içine temizlediğimiz numarayı ekliyoruz
            var payload = new
            {
                TelNo = formattedPhone,
                Message = message
            };

            //Özellik isimlerinin büyük/küçük harf yapısını bozmadan JSON'a çeviriyoruz
            var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            //ApiKey modelde olmadığı için HTTP Header üzerinden gönderiyoruz
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            try
            {
                //API'ye POST isteğini atıyoruz
                var response = await _httpClient.PostAsync(apiUrl, content);

                //Hata kontrolü
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"SMS Api Hatası: {response.StatusCode} - Detay: {errorDetail}");

                    throw new Exception($"SMS sağlayıcısı hata döndürdü: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{phoneNumber} numarasına SMS gönderilirken beklenmedik bir hata oluştu.");
                throw;
            }
        }
    }
}