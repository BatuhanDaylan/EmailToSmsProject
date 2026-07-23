using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{

    //email hesaplarını güncellerken kullanılacak method
    public class EmailAccountUpdateDto
    {
        [Required , EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        [Required]
        public string TenantId { get; set; } = string.Empty;
        [Required]
        public string ClientId { get; set; } = string.Empty;
        [Required]
        public string ClientSecret { get; set; } = string.Empty;
    }
}
