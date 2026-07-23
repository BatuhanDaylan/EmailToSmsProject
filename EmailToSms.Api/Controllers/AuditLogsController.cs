using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmailToSms.Api.Common;
using EmailToSms.Api.Models;

namespace EmailToSms.Api.Controllers
{
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
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(100)
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<AuditLog>>.Success(logs, "Log kayıtları listelendi."));
        }
    }
}