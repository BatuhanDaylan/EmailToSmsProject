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




