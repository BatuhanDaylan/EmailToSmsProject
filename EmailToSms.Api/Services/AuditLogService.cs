using EmailToSms.Api.Data;
using EmailToSms.Api.Models;
using EmailToSms.Api.DTOs;
using EmailToSms.Api.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        // SENİN MEVCUT LOG EKLEME METODUN (Değişmedi)
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

        // YENİ: LOGLARI SAYFALI BİR ŞEKİLDE GETİREN METOT
        public async Task<ResultOfT<PaginatedList<AuditLogDto>>> GetPaginatedLogsAsync(int page, int pageSize)
        {
            var totalCount = await _context.AuditLogs.CountAsync();

            var logs = await _context.AuditLogs
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AuditLogDto
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    EventType = x.EventType,
                    Details = x.Details,
                    IsSuccess = x.IsSuccess,
                    EmailAccountId = x.EmailAccountId, // Yeni eklenen alan
                    ForwardingRuleId = x.ForwardingRuleId // Yeni eklenen alan
                })
                .ToListAsync();

            var paginatedData = new PaginatedList<AuditLogDto>(logs, totalCount, page, pageSize);

            return ResultOfT<PaginatedList<AuditLogDto>>.Success(paginatedData, "İşlem logları başarıyla getirildi.", 200);
        }
    }
}