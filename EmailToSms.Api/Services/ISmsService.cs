namespace EmailToSms.Api.Services
{
    //Farlı SMS sağlayıcıları için ortak bir arayüz tanımladık
    public interface ISmsService
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}
