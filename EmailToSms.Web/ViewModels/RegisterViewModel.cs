using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6 , ErrorMessage = "Şifreniz en az altı karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
