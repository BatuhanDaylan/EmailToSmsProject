namespace EmailToSms.Api.Services
{
    public class MockSmsService : ISmsService
    {
        //geçici sms gönderme servisimiz
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"Mock SMS gönderildi: {phoneNumber} - {message}");
            return Task.CompletedTask;
        }
    }
}
