using Azure.Identity;
using Microsoft.Graph;
using EmailToSms.Api.Models;
using Microsoft.Graph.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection; // EKLENDİ

namespace EmailToSms.Api.Services
{
    public class MicrosoftGraphEmailReader : IMicrosoftGraphEmailReader
    {
        private readonly ISmsService _smsService;
        private readonly AuditLogService _auditLogService;
        private readonly IDataProtector _protector; // EKLENDİ

        // Constructor güncellendi
        public MicrosoftGraphEmailReader(ISmsService smsService, AuditLogService auditLogService, IDataProtectionProvider provider)
        {
            _smsService = smsService;
            _auditLogService = auditLogService;
            // Şifreyi çözerken Controller'da kullandığımız anahtar kelimenin BİREBİR AYNISINI kullanmalıyız
            _protector = provider.CreateProtector("EmailToSms.MicrosoftGraph.Secret");
        }

        //Yazdığımız bu method sayesinde email mesajlarının göndericisi, konusu ve içeriği ile ilgili kuralları kontrol ediyoruz
        public bool CheckIfEmailRuleMatches(Message emailMessage, ForwardingRule rule)
        {
            bool matches = true;

            if (!string.IsNullOrWhiteSpace(rule.SenderKeyword))
            {
                var sender = emailMessage.From?.EmailAddress?.Address ?? string.Empty;
                if (!sender.Contains(rule.SenderKeyword, StringComparison.OrdinalIgnoreCase)) matches = false;
            }

            if (!string.IsNullOrWhiteSpace(rule.SubjectKeyword))
            {
                var subject = emailMessage.Subject ?? string.Empty;
                if (!subject.Contains(rule.SubjectKeyword, StringComparison.OrdinalIgnoreCase)) matches = false;
            }

            if (!string.IsNullOrWhiteSpace(rule.BodyKeyword))
            {
                var body = emailMessage.BodyPreview ?? string.Empty;
                if (!body.Contains(rule.BodyKeyword, StringComparison.OrdinalIgnoreCase)) matches = false;
            }

            return matches;
        }

        public async Task ReadEmailAndProcessAsync(EmailAccount account)
        {
            try
            {
                // DÜZELTME: Veritabanındaki şifrelenmiş metni gerçek şifreye dönüştürüyoruz
                var plainSecret = _protector.Unprotect(account.ClientSecret);

                // Microsoft'a bağlanırken çözülmüş gerçek şifreyi veriyoruz
                var credential = new ClientSecretCredential(account.TenantId, account.ClientId, plainSecret);
                var graphClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });

                //graph apinin anlayabilmesi için tarihi ISO 8601 formatında alıp tek tırnak içine sarıyoruz
                // Tarihi alıyoruz (Microsoft Graph tarihi tırnaksız bekler)
                // Utc zaman diliminde olduğundan emin olmak için evrensel formata (Z) çeviriyoruz
                var lastCheckedIso = account.LastCheckedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var messages = await graphClient.Users[account.EmailAddress].Messages
                    .GetAsync(config =>
                    {
                        config.QueryParameters.Top = 10;
                        // lastCheckedIso değerini tek tırnak OLMADAN doğrudan ekliyoruz
                        config.QueryParameters.Filter = $"isRead eq false and receivedDateTime ge {lastCheckedIso}";
                        config.QueryParameters.Orderby = new[] { "receivedDateTime asc" };
                    });

                if (messages?.Value == null || messages.Value.Count == 0)
                {
                    return;
                }

                //Her bir mail için her bir kuralı kontrol ederiz
                foreach (var message in messages.Value)
                {
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
                                try
                                {
                                    await _smsService.SendSmsAsync(targetPhoneNumber.PhoneNumber, smsText);

                                    // YENİ: Başarılı SMS gönderimini veritabanına logluyoruz
                                    await _auditLogService.LogAsync(
                                        eventType: "SmsSent",
                                        details: $"{targetPhoneNumber.PhoneNumber} numarasına SMS iletildi. Sebep : gönderici '{rule.SenderKeyword}' , konu '{rule.SubjectKeyword}' , içerik '{rule.BodyKeyword}' kuralı tetiklendi.",
                                        isSuccess: true,
                                        accountId: account.Id,
                                        ruleId: rule.Id
                                    );
                                }
                                catch (Exception smsEx)
                                {
                                    // YENİ: Başarısız SMS gönderimini logluyoruz (Böylece sistem çökmüyor, diğer numaralara devam ediyor)
                                    await _auditLogService.LogAsync(
                                        eventType: "SmsFailed",
                                        details: $"{targetPhoneNumber.PhoneNumber} numarasına SMS gönderilemedi! Hata: {smsEx.Message}",
                                        isSuccess: false,
                                        accountId: account.Id,
                                        ruleId: rule.Id
                                    );
                                }
                            }
                        }
                    }

                    //Mesaj okunduyssa işaretleriz böylece tekrar işleme alınmaz
                    var requestBody = new Message
                    {
                        IsRead = true
                    };

                    // güvenlik: message.Id null olabilir -> kontrol et
                    if (!string.IsNullOrWhiteSpace(message.Id))
                    {
                        await graphClient.Users[account.EmailAddress].Messages[message.Id].PatchAsync(requestBody);
                    }
                }

                account.LastCheckedAt = DateTime.UtcNow;
            }
            //Oluşan hataları catch ile yakalıyoruz
            catch (Exception ex)
            {
                Console.WriteLine($"Email okunurken hata oluştu: {ex.Message}");

                // YENİ: Graph API'ye bağlanırken veya mailleri çekerken oluşan kritik hataları logluyoruz
                await _auditLogService.LogAsync(
                    eventType: "GraphApiError",
                    details: $"Graph API'den {account.EmailAddress} mailleri okunurken hata: {ex.Message}",
                    isSuccess: false,
                    accountId: account.Id
                );
            }
        }
    }
}