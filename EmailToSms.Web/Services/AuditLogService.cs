using EmailToSms.Web.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EmailToSms.Web.Services
{
    public class AuditLogService
    {
        private readonly HttpClient _httpClient;

        public AuditLogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AuditLogViewModel>> GetLogsAsync()
        {
            var response = await _httpClient.GetAsync("api/AuditLogs");

            if (response.IsSuccessStatusCode)
            {
                // DÜZELTME: API'den gelen veriyi ApiResponse zarfıyla karşılıyoruz
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<AuditLogViewModel>>>();

                // Zarfın içindeki Data kısmını (Listeyi) dönüyoruz, boşsa boş liste dönüyoruz
                return apiResponse?.Data ?? new List<AuditLogViewModel>();
            }

            return new List<AuditLogViewModel>();
        }
    }
}