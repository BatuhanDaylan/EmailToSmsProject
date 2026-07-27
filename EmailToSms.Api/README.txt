Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools, Npgsql.EntityFrameworkCore.PostgreSQ paketleri kuruldu
E-Posta bilgilerini ve Microsoft 365 bağlantı bilgilerini tutan EmailAccount yazıldı
E-Posta kurallarının saklandığı ForwardingRule eklendi
Veritabanımızla uygulamamız arasında iletişim kurmamızı sağlayacak AppDbContext yazıldı
Microsoft.Graph, Azure.Identity paketleri kuruldu
Farklı gsm operatörlerine implementasyon yapılabilmesi için ISmsService interface'i yazıldı
Geçici olarak mesaj gönderimi yapacak MockSmsService yazıldı
MicrosoftGraphEmailReader servisi içine mail kurallarını kontrol eden yapı yazıldı
MicrosoftGraphEmailReader servisi içine API çağrıları yapmamızı sağlayacak yapı yazıldı
MicrosoftGraphEmailReader servisi içine son okunmamış mailin bilgisini tutacak yapı yazıldı
MicrosoftGraphEmailReader servisi içine gelen okunmamış mailleri tek tek kontrol edip kurallara göre sms gönderimi yapan yapı yazıldı -Düzenleme kontrol edilen mailler okundu olarak işaretlenmiyor ve sürekli tekrar konrol ediliyordu bu problem çözüldü
AppUser modeli eklendi
AppUser modeli için AppDbContext içerisine identity framework ilişkisi eklendi
appsettings içerisine postgre sql bağlantı stringi eklendi
Microsoft.Api paketi kararlı eski sürümüne düşürüldü
Database oluşturuldu ve migration komutlarıyla tablolar oluşturuldu
Data Tranfer Object yapısı eklendi
Projemdeki veritabanını Identity ve Uygulama olarak ayırmamın mimariye daha uygun olacağı söylendi bu nedenle
Migration komutları dosyası silindi
drop-Database komutu ile veritabanı temizlendi
AppDbContext sınıfı silindi ve Identity ile uygulama kısmı için iki ayrı DbContext eklendi
Program.cs içindeki IdentityDbContext ve DbContext ekleme kodları düzenlendi 
Projemdeki bir yanlışıda ben farkettim telefon numaraları kurallar içinde bir listede tutuluyordu artık telefon numaraları için ayrı bir sınıf yazıldı
Migration komutları ile Uygulama ve Identity için iki farklı veritabanı oluşturuldu
Controllers içerisine authorization ve JWT işlemlerini yapacak AuthController sınıfı eklendi
AuthController içerisine kullanıcı kayıt işlemlerini yapacak yapı yazıldı
AuthController içerisine kullanıcı giriş işlemleri, token işlemlerini yapacak yapı yazıldı
Kullanıcının email eklerken kullanacağı DTO yazıldı
Kullanıcıya eklediği mailleri listeleyeceğimiz yapı yazıldı
Mail hesaplarının işlemlerinin yapılacağı EmailAccountsController eklendi mail kayıt eden method yazıldı
Girilmiş mail hesaplarını listeleyecek olan method yazıldı
İstenen mail hesabını bulup silecek method yazıldı
Kullanıcının kural eklerken kullanacağı DTO ve kural listelerken kullanacağı DTO yazıldı
Kural ve telefon numarası işlemlerinin yapılacağı ForwardingRulesController eklendi
Kural ve telefon numaraları eklenmesini sağlayacak method yazıldı
Kuralların ve telefon numaralarının listelenmesini sağlayacak method yazıldı
Kuralların ve telefon numaralarının silinmesini sağlayacak methot yazıldı
EmailAccountsController içerisine email güncelleme methodu eklende ilgili DTOsu da eklendi
ForwardingRulesController içerisindeki telefon numaraları ile ilgili methodlar çıkarıldı kural güncelleme metodu ve ilgili DTOsu eklendi
Telefon numaraları ekleme listeleme güncelleme ve silme methodları için PhoneNumbersController ve gerekli DTOları eklendi
Telefon numaralarının eklenmesini sağlayan method eklendi
Telefon numaralarının listelenmesini güncellenmesini ve silinmesini sağlayan methodlar eklendi
Mesajları şu anlık konsol ekranına yazdıracak SmsService sınıfı yazıldı
Arka planda çalışacak olan EmailCheckerWorker servisi yazıldı
IMicrosoftGraphEmailReader servisi yazıldı
MicrosoftGraphEmailReader servisi IMigrosoftGraphEmailReader servisine göre düzenlendi ve kod iyileştirmeleri yapıldı
Program.cs içerisindeki kodlar swagger a göre yeniden düzenlendi bu kısımda Microsoft.OpenApi paketi ile swagger için gerekli olan Swashbuckle.AspNetCore paketi çakıştı hatanın tespiti ve çözümü çok uzun sürdü ancak sonunda iki pakette uyumlu çalışabilecekleri daha eski kararlı sürümlere çekildi ve sorun çözüldü
EmailCheckerWorker services klasörünün içerisinden workers klasörüne taşındı
MicrosoftGraphEmailReader içerisinde graph apinin anlayabilmesi için tarihi ISO 8601 formatına alındı
DTO larda eksik olan required kısımları eklendi
Web katmanı için mvc web projesi oluşturuldu
token bağlantısı için TokenHandler servisi yazıldı ve program.cs içerisine gerekli kodlar eklendi
Diğer classların miras alacağı bir abstract class eklendi içerisine CreatedAt IsDeleted IsActive propları yazıldı
RulePhoneNumber içerindeki kurallara bağımlılık kaldırılıyor çoklu ilişki kuruldu
PhoneNumbersController kurallar ve numaralar arasındaki bağımlılık kaldırıldığı için çoka çok ilişkiye göre düzenlendi
Common klasörü eklendi içerisine hata kontrolü için gerekli Result ve ResultOfT(veri taşıyan) eklendi içerisine hatanın durumlarını tutan ve aynı zamanda veri taşıyan kodlar yazıldı
Middlewares klasörü eklendi içerisine ExceptionHandlingMiddleware middleware eklendi ve içerisine hata yönetimi yapacak kodlar yazıldı
Program.cs içerisine middleware eklendi
Controller içerisindeki methodların Ok gibi belirsiz responslar döndürmemesi için controller içerisine CustumBaseController eklendi gerekli kodlaar yazıldı
AuthController içerisindeki methotlar Result döndürecek şekilde düzenlendi
EmailAccountsController içerisindeki methotlar Result döndürecek şekilde düzenlendi
ForwardingRulesController içerisindeki methotlar Result döndürecek şekilde düzenlendi
PhoneNumbersController içerisindeki methotlar Result döndürecek şekilde düzenlendi
StatsController içerisindeki methotlar Result döndürecek şekilde düzenlendi
AuditlogsController içerisindeki methotlar Result döndürecek şekilde düzenlendi
Controllerlar içerisindeki silme methodları veriler veritabanından silinmeyecek sadece deaktif hale alınacak şekilde düzenlendi
Controllerlar içerisindeki create methodlarına daha önce eklenmişmi kontrolü eklendi
PhoneNumberController içerisine kurala bağlı rehberi getirme, kurala bağlama, bağlı kuraldan silme gibi yeni merhotlar eklendi
RulePhoneNumber içerisine UserId eklendi
PhoneRuleAssignDto eklendi
AuditLogControllerımız tüm kullanıcıların loglarını getiriyordu şimdi sadece giriş yapmış kullanıcının loglarını getirecek şekilde güncellendi

Refactoring işlemleri başlatıldı
EmailCheckerWorkerda döngünün içindeki işlemleri kendi try-catch bloğuna aldık. Böylece bir hesap hata alsa bile döngü çalışmaya devam eder
MicrosoftGraphEmailReaderd bellek dostu ve profesyonel standart olan StringComparison.OrdinalIgnoreCase yapısına geçildi ve null kontrolleri ?? string.Empty ile daha güvenli hale getirildi
Program.cs içerisine DataProrotection eklendi
EmailAccountsController içerisine ekleme ve güncelleme methodu içerisindeci clientsecret değeri veritabanına yollanmadan önce şifreleyecek kod eklendi
MicrosoftGraphEmailReader Worker mailleri okumak için Microsoft'a bağlanacağı zaman veritabanından gelen bu kriptolu metni Microsoft Graph ın anlayabileceği düz metne çevirmelidir kodlar buna göre güncellendi









