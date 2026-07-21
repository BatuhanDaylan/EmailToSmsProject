using EmailToSms.Api.Data;
using EmailToSms.Api.Models;
using System;
using System.Threading.Tasks;

namespace EmailToSms.Api.Services
{
    public class AuditLogService
    {
        private readonly EmailToSmsDbContext _context;

        public AuditLogService(EmailToSmsDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string eventType, string details, bool isSuccess, int? accountId = null, int? ruleId = null)
        {
            var log = new AuditLog
            {
                EventType = eventType,
                Details = details,
                IsSuccess = isSuccess,
                EmailAccountId = accountId,
                ForwardingRuleId = ruleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}