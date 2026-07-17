using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Web.ViewModels
{
    public class ForwardingRuleCreateViewModel
    {
        [Required(ErrorMessage = "Bu kuralın hangi e-posta hesabına ait olduğunu seçmelisiniz.")]
        public int EmailAccountId { get; set; }
        public string? SenderKeyword { get; set; }
        public string? SubjectKeyword { get; set; }
        public string? BodyKeyword { get; set; }
    }
}
