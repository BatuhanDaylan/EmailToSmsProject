using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class EmailAccountUpdateService
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string EmailAddress { get; set; } = string.Empty;
        [Required(ErrorMessage = "TentantId zorunludur.")]
        public string TentantId { get; set; } = string.Empty;
        [Required(ErrorMessage = "ClientId zorunludur.")]
        public string ClientId { get; set; } = string.Empty;
        [Required(ErrorMessage = "ClientSecret zorunludur.")]

        public string ClientSecret { get; set; } = string.Empty;
    }
}
