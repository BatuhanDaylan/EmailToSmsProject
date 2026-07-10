namespace EmailToSms.Api.DTOs
{

    //email hesaplarını güncellerken kullanılacak method
    public class EmailAccountUpdateDto
    {
        public string EmailAddress { get; set; } = string.Empty;
        public string TentantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
