namespace EmailToSms.Api.DTOs
{
    public class ForwardingRuleUpdateDto
    {
        public string? SenderKeyword { get; set; }
        public string? SubjectKeyword { get; set; }
        public string? BodyKeyword { get; set; }
    }
}
