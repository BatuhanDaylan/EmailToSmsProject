namespace EmailToSms.Api.Models
{
    //Gelen e-postaları filtrelemek için kullandığımız esnek kural sınıfı
    public class ForwardingRule : BaseEntity
    {
        public int Id { get; set; }

        //Kullanıcı hangisini doldurursa ona göre arama yapılır.
        public string? SenderKeyword { get; set; }
        public string? SubjectKeyword { get; set; }
        public string? BodyKeyword { get; set; }  

        //Kural eşleştiğinde mesaj atılacak telefon numaraları listesi
        public ICollection<RulePhoneNumber> TargetPhoneNumbers { get; set; } = new List<RulePhoneNumber>();

        //Bu kuralın hangi e-posta hesabına ait olduğunu belirten ilişki
        public int EmailAccountId { get; set; }
        public EmailAccount? EmailAccount { get; set; }
    }
}