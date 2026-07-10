namespace EmailToSms.Api.DTOs
{
    public class PhoneNumberCreateDto
    {
        public int ForwardingRuleId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
