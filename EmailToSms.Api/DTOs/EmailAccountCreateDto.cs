using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{

    //email eklenirken kullanılacak sınıf
    public class EmailAccountCreateDto
    {
        [Required , EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        [Required]
        public string TentantId { get; set; } = string.Empty;
        [Required]
        public string ClientId { get; set; } = string.Empty;
        [Required]
        public string ClientSecret { get; set; } = string.Empty;
    }
}
