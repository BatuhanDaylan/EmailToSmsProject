using EmailToSms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmailToSms.Web.Controllers
{
    public class LogsController : Controller
    {
        private readonly AuditLogService _auditLogService;

        public LogsController(AuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _auditLogService.GetLogsAsync();
            return View(logs);
        }
    }
}