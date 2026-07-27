using Microsoft.EntityFrameworkCore;
using EmailToSms.Api.Data;
using EmailToSms.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmailToSms.Api.Workers
{
    public class EmailCheckerWorker : BackgroundService
    {
        private readonly ILogger<EmailCheckerWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        //Dependency injection kullanarak projemize entegre ediyoruz
        public EmailCheckerWorker(ILogger<EmailCheckerWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Arka plan e-posta kontrol işçisi başlatıldı");

            //uygulama durdurulana kadar döngü devam eder
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"E-postalar kontrol ediliyor. Zaman : {DateTime.Now:HH:mm:ss}");

                try
                {
                    //scoped yaşam döngüsü her http istek başına yeni nesne üretir
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        //kapsam içinden veritabanı bağlantımızı güvenle çekiyoruz
                        var dbContext = scope.ServiceProvider.GetRequiredService<EmailToSmsDbContext>();
                        var emailReaderService = scope.ServiceProvider.GetRequiredService<IMicrosoftGraphEmailReader>();

                        // YENİ: Loglama servisimizi scope üzerinden çekiyoruz
                        var auditLogService = scope.ServiceProvider.GetRequiredService<AuditLogService>();

                        //mail hesaplarının kurallarını ve telefon numaralarını çekiyoruz
                        var accounts = await dbContext.EmailAccounts
                            .Include(e => e.Rules)
                            .ThenInclude(r => r.TargetPhoneNumbers)
                            .ToListAsync(stoppingToken);

                        _logger.LogInformation($"Sistemde kayıtlı toplam {accounts.Count} e-posta hesabı bulundu");

                        foreach (var account in accounts)
                        {
                            _logger.LogInformation($"Kontrol edilen hesap {account.EmailAddress}");

                            await emailReaderService.ReadEmailAndProcessAsync(account);

                            account.LastCheckedAt = DateTime.UtcNow;

                            // YENİ: Her bir hesabın başarıyla tarandığını veritabanına işliyoruz
                            await auditLogService.LogAsync(
                                eventType: "WorkerScan",
                                details: $"{account.EmailAddress} hesabı başarıyla tarandı.",
                                isSuccess: true,
                                accountId: account.Id
                            );
                        }
                        //değişiklikleri kaydet
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "E-posta kontrolü döngüsü sırasında bir hata yaşandı");

                    // YENİ: Kritik hataları veritabanına (arayüze düşmesi için) işliyoruz
                    try
                    {
                        using (var errorScope = _scopeFactory.CreateScope())
                        {
                            var errorAuditService = errorScope.ServiceProvider.GetRequiredService<AuditLogService>();
                            await errorAuditService.LogAsync(
                                eventType: "WorkerError",
                                details: $"Worker döngüsünde kritik hata: {ex.Message}",
                                isSuccess: false
                            );
                        }
                    }
                    catch (Exception logEx)
                    {
                        // Log atarken hata olursa sadece konsola yaz (Döngüyü kırma)
                        _logger.LogError(logEx, "Sistem hatası veritabanına loglanırken başarısız oldu!");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("Arka plan e-posta kontrol işçisi durduruldu");
        }
    }
}