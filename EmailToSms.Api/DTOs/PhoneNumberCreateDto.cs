using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{
    public class PhoneNumberCreateDto
    {
        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(@"^(\+90|0)?5[0-9]{9}$", ErrorMessage = "Lütfen geçerli bir telefon numarası giriniz. (Örn: 05551234567)")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}