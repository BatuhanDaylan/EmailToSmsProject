using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmailToSms.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailAccountService _emailAccountService;
        private readonly AuditLogService _auditLogService;

        public HomeController(EmailAccountService emailAccountService, AuditLogService auditLogService)
        {
            _emailAccountService = emailAccountService;
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index()
        {
            // API'den dashboard istatistiklerini al
            // eğer GetStatsAsync farklıysa buna göre düzenleyin
            var stats = await _emailAccountService.GetStatsAsync();

            ViewBag.AccountCount = stats.AccountCount;
            ViewBag.RuleCount = stats.RuleCount;
            ViewBag.SmsCount = stats.SmsSentCount;

            // Son logları al (API'den) ve en fazla 5 tanesini ViewBag'e koy
            var logs = await _auditLogService.GetLogsAsync();
            var recent = logs?.Take(5).ToList() ?? new List<AuditLogViewModel>();
            ViewBag.RecentLogs = recent;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}