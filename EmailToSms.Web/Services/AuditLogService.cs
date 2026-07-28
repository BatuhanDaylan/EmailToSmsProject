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
                // DÜZELTME: API'den gelen verinin "data" kısmı bir List değil, PaginatedList objesidir.
                // Bu yüzden ApiResponse'un içindeki T tipini PaginatedList olarak veriyoruz.
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<AuditLogViewModel>>>();

                // Zarfın içindeki Data kısmından asıl listeyi (Items) dönüyoruz.
                // Eğer ApiResponse veya Data null ise boş bir liste dönüyoruz.
                return apiResponse?.Data?.Items ?? new List<AuditLogViewModel>();
            }

            return new List<AuditLogViewModel>();
        }

        public async Task<PaginatedList<AuditLogViewModel>> GetPaginatedLogsAsync(int page = 1)
        {
            var response = await _httpClient.GetAsync($"api/AuditLogs?page={page}");

            if (response.IsSuccessStatusCode)
            {
                // Burada veriyi List değil, sayfalama yapısıyla (PaginatedList) alıyoruz
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<AuditLogViewModel>>>();

                // Veri başarıyla geldiyse Data'nın kendisini (PaginatedList objesini) dönüyoruz
                return apiResponse?.Data ?? new PaginatedList<AuditLogViewModel> { Items = new List<AuditLogViewModel>() };
            }

            // Hata durumunda boş bir PaginatedList dönüyoruz ki View patlamasın
            return new PaginatedList<AuditLogViewModel> { Items = new List<AuditLogViewModel>() };
        }
    }
}