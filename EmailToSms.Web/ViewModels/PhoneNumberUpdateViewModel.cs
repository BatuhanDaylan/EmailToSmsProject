using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class PhoneNumberUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ForwardingRuleId { get; set; }

        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        // DÜZELTME: Ekleme sayfasındaki aynı doğrulama kuralını buraya da ekledik.
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Lütfen geçerli bir telefon numarası girin.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}