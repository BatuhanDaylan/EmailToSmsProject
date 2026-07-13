using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{
    public class PhoneNumberCreateDto
    {
        [Required]
        public int ForwardingRuleId { get; set; }
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
