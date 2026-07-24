using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class PhoneNumberCreateViewModel
    {
        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Lütfen geçerli bir telefon numarası girin.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}