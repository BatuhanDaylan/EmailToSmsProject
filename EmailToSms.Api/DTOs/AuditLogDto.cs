using System;

namespace EmailToSms.Api.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public int? EmailAccountId { get; set; }
        public int? ForwardingRuleId { get; set; }
    }
}