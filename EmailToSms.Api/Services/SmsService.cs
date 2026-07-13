namespace EmailToSms.Api.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<ISmsService> _logger;

        public SmsService(ILogger<ISmsService> logger)
        {
            _logger = logger; ;
        }

        //konsol ekranına mesaj gönderecek olan sms simulasyonu
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("=============================");
            _logger.LogInformation($"Hedef Numara : {phoneNumber}");
            _logger.LogInformation($"Mesaj : \n {message}");
            _logger.LogInformation("=============================");
            return Task.CompletedTask;
        }


    }
}
