namespace EmailToSms.Api.DTOs
{

    //email eklenirken kullanılacak sınıf
    public class EmailAccountCreateDto
    {
        public string EmailAdress { get; set; } = string.Empty;
        public string TentantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
