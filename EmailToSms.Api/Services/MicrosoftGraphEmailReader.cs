using Azure.Identity;
using Microsoft.Graph;
using EmailToSms.Api.Models;
using Microsoft.Graph.Models;

namespace EmailToSms.Api.Services
{
    public class MicrosoftGraphEmailReader : IMicrosoftGraphEmailReader
    {
        private readonly ISmsService _smsService;
        public MicrosoftGraphEmailReader(ISmsService smsService)
        {
            _smsService = smsService;
        }


        //Yazdığımız bu method sayesinde email mesajlarının göndericisi, konusu ve içeriği ile ilgili kuralları kontrol ediyoruz
        public bool CheckIfEmailRuleMatches(Message emailMessage,ForwardingRule rule)
        {
            bool matches = true;

            if(!string.IsNullOrWhiteSpace(rule.SenderKeyword))
            {
                var sender = emailMessage.From?.EmailAddress?.Address?.ToLower() ?? "";
                if (!sender.Contains(rule.SenderKeyword.ToLower())) matches = false;
            }
            if (!string.IsNullOrWhiteSpace(rule.SubjectKeyword))
            {
                var subject = emailMessage.Subject?.ToLower() ?? "";
                if (!subject.Contains(rule.SubjectKeyword.ToLower())) matches = false;
            }

            if (!string.IsNullOrWhiteSpace(rule.BodyKeyword))
            {
                var body = emailMessage.BodyPreview?.ToLower() ?? "";
                if (!body.Contains(rule.BodyKeyword.ToLower())) matches = false;
            }
            return matches;
        }

        public async Task ReadEmailAndProcessAsync(EmailAccount account)
        {
            //Azure portal üzerinden uygulama kaydı oluşturulduktan sonra Graph API'ye erişim sağlamak için gerekli olan anahtarı alan bir nesne oluşturuyoruz
            var credential = new ClientSecretCredential(account.TenantId, account.ClientId, account.ClientSecret);
            //Graph API ile iletişim kurmak için GraphServiceClient sınıfını kullanıyoruz Microsoft Graph API'sine erişim sağlamak için gerekli olan kimlik doğrulama bilgilerini alır ve API çağrılarını yapmamıza olanak tanır
            var graphClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });

            try
            {
                //graph apinin anlayabilmesi için tarihi evrensel formata çevirme
                string lastCheckedString = account.LastChecketAt.ToString("yyyy-MM-ddTHH:mm:ssZ");

                //En üstteki 10 okunmamış maili alırız 
                //Geliş tarihi son kontrol edilenden sonra ise maili alırız
                //En eski meilden yeni maile göre sıralarız
                var messages = await graphClient.Users[account.EmailAddress].Messages
                    .GetAsync(config =>
                    {
                        config.QueryParameters.Top = 10;
                        config.QueryParameters.Filter = $"isRead eq false and receivedDateTime ge {lastCheckedString}";
                        config.QueryParameters.Orderby = new[] { "receivedDateTime asc" };
                    });

                //Her bir mail için her bir kuralı kontrol ederiz
                foreach (var message in messages.Value)
                {
                    bool wasSmsSent = false;
                    foreach (var rule in account.Rules)
                    {
                        //Eğer mail kurallara uyuyorsa sms göndeririz
                        if (CheckIfEmailRuleMatches(message, rule))
                        {
                            string tarih = message.ReceivedDateTime?.ToString("dd.MM.yyyy HH:mm") ?? "Bilinmeyen Tarih";
                            string gonderici = message.From?.EmailAddress?.Address ?? "Bilinmeyen Gönderici";
                            string konu = message.Subject ?? "Konusuz";
                            string smsText = $"Tarih: {tarih}\nGönderici: {gonderici}\nKonu: {konu}";

                            foreach (var targetPhoneNumber in rule.TargetPhoneNumbers)
                            {
                                await _smsService.SendSmsAsync(targetPhoneNumber.PhoneNumber, smsText);
                            }
                        }
                    }

                    //Mesaj okunduyssa işaretleriz böylece tekrar işleme alınmaz
                    var requestBody = new Message
                    {
                        IsRead = true
                    };
                    await graphClient.Users[account.EmailAddress].Messages[message.Id].PatchAsync(requestBody);
                }
                //burada bir yazım yanlışı var düzeltilmeli
                account.LastChecketAt = DateTime.UtcNow;

            }
            //Oluşan hataları cacth ile yakalıyoruz
            catch (Exception ex) 
            {
                Console.WriteLine($"Email okunurken hata oluştu{ex.Message}");
            }
        }
    }
}
