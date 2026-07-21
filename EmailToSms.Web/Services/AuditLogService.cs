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
            //API adresine göre endpoint'i çağırıyoruz
            var logs = await _httpClient.GetFromJsonAsync<List<AuditLogViewModel>>("api/AuditLogs");
            return logs ?? new List<AuditLogViewModel>();
        }
    }
}