namespace EmailToSms.Api.Models
{
    public class RulePhoneNumber : BaseEntity
    {
        public int Id { get; set; }

        //Hedef telefon numarası
        public string PhoneNumber { get; set; } = string.Empty;

        //çoklu ilişki kuruyoruz
        public ICollection<ForwardingRule> ForwardingRules { get; set; } = new List<ForwardingRule>();

    }
}
