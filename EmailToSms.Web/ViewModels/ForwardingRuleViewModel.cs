namespace EmailToSms.Web.ViewModels
{
    public class ForwardingRuleViewModel
    {
        public int Id { get; set; }
        public int EmailAccountId { get; set; }
        public string? SenderKeyword { get; set; }
        public string? SubjectKeyword { get; set; }
        public string? BodyKeyword { get; set; }
    }
}
