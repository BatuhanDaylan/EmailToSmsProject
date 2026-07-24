namespace EmailToSms.Api.Models
{
    public class RulePhoneNumber : BaseEntity
    {
        public int Id { get; set; }

        //kullanıcının Id değerini tutar
        public string UserId { get; set; } = string.Empty;

        //Hedef telefon numarası
        public string PhoneNumber { get; set; } = string.Empty;

        //çoklu ilişki kuruyoruz
        public ICollection<ForwardingRule> ForwardingRules { get; set; } = new List<ForwardingRule>();

    }
}
