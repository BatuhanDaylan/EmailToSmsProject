using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{
    public class PhoneNumberUpdateDto
    {
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
