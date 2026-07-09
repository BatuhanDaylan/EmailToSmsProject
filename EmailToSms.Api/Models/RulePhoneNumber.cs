namespace EmailToSms.Api.Models
{
    public class RulePhoneNumber
    {
        public int Id { get; set; }

        //Hedef telefon numarası
        public string PhoneNumber { get; set; } = string.Empty;

        //Hangi kurala ait
        public int ForwardingRuleId { get; set; }
        public ForwardingRule? ForwardingRule { get; set; }

    }
}
