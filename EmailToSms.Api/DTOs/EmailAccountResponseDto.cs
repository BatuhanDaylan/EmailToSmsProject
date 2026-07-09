namespace EmailToSms.Api.DTOs
{

    //kullanıcıya eklenen mailleri listelerken göstereceğimiz yapı
    public class EmailAccountResponseDto
    {
        public int Id { get; set; }
        public string EmailAdress { get; set; } = string.Empty;
        public DateTime LastChecked { get; set; } 
    }
}
