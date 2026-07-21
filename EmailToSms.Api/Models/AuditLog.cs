using System;

namespace EmailToSms.Api.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        //olayın gerçekleştiği an 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //olayın tipi 
        public string EventType { get; set; } = string.Empty;

        //Kullanıcıya gösterilecek detay mesajı
        public string Details { get; set; } = string.Empty;

        //İşlem başarılı mı? 
        public bool IsSuccess { get; set; }

        //Olay belirli bir E-Posta hesabı ile ilgiliyse ID'sini tutarız
        public int? EmailAccountId { get; set; }
        public EmailAccount? EmailAccount { get; set; }

        //Olay belirli bir kuralı tetiklediyse ID'sini tutarız 
        public int? ForwardingRuleId { get; set; }
        public ForwardingRule? ForwardingRule { get; set; }
    }
}