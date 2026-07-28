using EmailToSms.Api.Common;
using EmailToSms.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace EmailToSms.Api.Controllers
{
    // Giriş yapmayı zorunlu kıldık
    [Authorize]
    [Route("api/[controller]")] // Route eksikti, ekledim.
    public class AuditLogsController : CustomBaseController
    {
        // Artık DbContext yerine doğrudan senin servisini kullanıyoruz
        private readonly AuditLogService _auditLogService;

        public AuditLogsController(AuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] int page = 1)
        {
            int pageSize = 20; // Bir sayfada kaç kayıt olacak

            // Tüm yükü servise devrettik
            var result = await _auditLogService.GetPaginatedLogsAsync(page, pageSize);

            return CreateActionResult(result);
        }
    }
}