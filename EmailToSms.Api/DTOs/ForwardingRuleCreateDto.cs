namespace EmailToSms.Api.DTOs
{
    public class ForwardingRuleCreateDto
    {
        public int EmailAccountId { get; set; }
        public string? SenderKeyword { get; set; }
        public string? BodyKeyword { get; set; }
        public string? SubjectKeyword { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();


    }
}
