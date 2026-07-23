using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly EmailToSmsDbContext _context;
        public StatsController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetMyStats()
        {
            var userId = GetUserId();

            // Kullanıcıya ait hesap Id'leri
            var accountIds = await _context.EmailAccounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            var accountCount = accountIds.Count;

            // Kuralların sayısı (kullanıcının tüm hesaplarına bağlı)
            var ruleCount = await _context.ForwardingRules
                .CountAsync(r => accountIds.Contains(r.EmailAccountId));

            var smsSentCount = await _context.AuditLogs
                .CountAsync(l => l.EventType == "SmsSent" && l.IsSuccess && l.EmailAccountId.HasValue && accountIds.Contains(l.EmailAccountId.Value));

            return Ok(new
            {
                AccountCount = accountCount,
                RuleCount = ruleCount,
                SmsSentCount = smsSentCount
            });
        }
    }
}