namespace EmailToSms.Api.Models
{
    // E-posta hesaplarını ve Microsoft 365 bağlantı bilgilerini tutan ana sınıf
    public class EmailAccount : BaseEntity
    {
        public int Id { get; set; }

        //identity veritabanıyla ilişkiye giren kullanıcı kimliği 
        public string UserId { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;

        // Microsoft Graph API kimlik bilgileri
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        //Milad tarihini tutan özellik
        public DateTime LastCheckedAt { get; set; } = DateTime.UtcNow;

        //Bir e-posta hesabına bağlı birden fazla kural olabilir
        public List<ForwardingRule> Rules { get; set; } = new List<ForwardingRule>();
    }
}