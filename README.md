# EmailToSms

Belirli e-posta hesaplarını arka planda dinleyip, tanımlanan yönlendirme
kurallarına uyan yeni e-postaları otomatik olarak SMS'e çevirip ilgili
telefon numaralarına gönderen bir .NET uygulaması.

## Nasıl çalışır

1. Kullanıcı bir e-posta hesabını (Microsoft/Outlook, Graph API ile OAuth)
   sisteme bağlar.
2. Hesap için **yönlendirme kuralları** tanımlanır: hangi e-postalar (örn.
   gönderen/konu eşleşmesi) hangi telefon numaralarına SMS olarak gitsin.
3. Arka planda çalışan `EmailCheckerWorker`, aktif e-posta hesaplarını
   periyodik olarak tarar; `MicrosoftGraphEmailReader` ile Microsoft Graph
   üzerinden yeni e-postaları okur.
4. Eşleşen e-postalar `CompanySmsService` aracılığıyla harici bir SMS
   sağlayıcısına gönderilir.
5. Her işlem (e-posta okuma, SMS gönderimi, hata) denetim kaydı (audit log)
   olarak tutulur ve web panelinden sayfalanmış şekilde izlenebilir.

## Mimari

İki ayrı .NET projesinden oluşur:

```
EmailToSms.Api/   REST API — Auth (JWT + ASP.NET Core Identity), e-posta hesabı/
                  yönlendirme kuralı/telefon numarası CRUD'u, arka plan worker'ı,
                  audit log, rate limiting (login'e karşı brute-force koruması)
EmailToSms.Web/   MVC panel — API'yi tüketen kullanıcı arayüzü (giriş/kayıt,
                  e-posta hesapları, yönlendirme kuralları, telefon rehberi, loglar)
```

- **Auth**: JWT tabanlı, ASP.NET Core Identity üzerine kurulu; login endpoint'i
  rate-limit korumalı (1 dakikada 5 başarısız denemeden sonra kilitlenir).
- **Gizli veri koruma**: e-posta hesabı client secret'ları veritabanına yazılmadan
  önce `DataProtection` ile şifrelenir; Worker, Microsoft Graph'a bağlanmadan önce
  bu şifreli metni çözer.
- **Veritabanı**: PostgreSQL + Entity Framework Core.
- **Gerçek sırlar** (JWT key, bağlantı dizeleri, SMS API bilgileri) repoya
  değil, .NET **User Secrets**'a girilir — `appsettings.json` yalnızca
  placeholder değerler içerir.

## Kurulum

```bash
# Her iki proje için de gerçek sırları User Secrets'a gir:
cd EmailToSms.Api
dotnet user-secrets set "Jwt:Key" "..."
dotnet user-secrets set "ConnectionStrings:IdentityConnection" "Host=...;Database=...;Username=...;Password=..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..."
dotnet user-secrets set "SmsSettings:ApiUrl" "..."
dotnet user-secrets set "SmsSettings:ApiKey" "..."

dotnet ef database update
dotnet run
```

```bash
cd EmailToSms.Web
dotnet run
```

Web panel, `appsettings.json`'daki `Api:BaseUrl` üzerinden API'ye bağlanır.

## Test

```bash
dotnet test
```
