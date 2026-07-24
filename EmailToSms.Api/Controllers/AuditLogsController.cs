using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EmailToSms.Api.Common;
using EmailToSms.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmailToSms.Api.Controllers
{
    // Giriş yapmayı zorunlu kıldık
    [Authorize]
    public class AuditLogsController : CustomBaseController
    {
        private readonly EmailToSmsDbContext _context;

        public AuditLogsController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            //adece bu kullanıcıya ait hesapların loglarını getiriyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var logs = await _context.AuditLogs
                .Include(l => l.EmailAccount) //E-Posta hesabı ilişkisini dahil et
                .Where(l => l.EmailAccount != null && l.EmailAccount.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(100)
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<AuditLog>>.Success(logs, "Log kayıtları listelendi."));
        }
    }
}