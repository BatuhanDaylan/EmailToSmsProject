using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{
    public class PhoneNumberCreateDto
    {
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
