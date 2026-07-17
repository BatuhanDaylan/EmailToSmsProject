using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class PhoneNumberUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ForwardingRuleId { get; set; }

        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}