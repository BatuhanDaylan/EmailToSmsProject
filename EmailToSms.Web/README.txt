Web sitemiz api ile iltişim kurmak istediğinde çalışacak olan TokenHandler servisi yazıldı
TokenHandler sınıfının çalışabilmesi için kullanıcının giriş yapması tokenın sessiona kaydedilmesi gerekmektedir bu nedenle AuthService yazıldı giriş kayıt çıkış işlemleri eklendi
LoginViewModel ve RegisterViewModel eklendi
AuthController eklendi AuthController içerisine login get post methodları eklendi
AuthController içerisine register get post methodları eklendi
AuthController içerisine logout methodu eklendi
ViewModels içerisine LoginViewModel ve RegisterViewModel eklendi 
Views/Auth içerisine Login ve Register eklendi
İki projenin portları düzenlendi
Çıkan hatalar düzeltildi
EmailAccountCreateViewModel eklendi
EmailAccountVieWModel eklendi
EmailAccountUpdateViewModel eklendi
EmailAccountService eklendi içerisine apiden veri çekip listeleyecek method yazıldı
EmailAccountService içerisine girilen verileri apiye yollayacak method yazıldı
EmailAccountService içerisine güncellenen verileri apiye yollayacak method yazıldı
EmailAccountService içerisine ver silen method yazıldı
EmailAccountController eklendi içerisine listeleme yapan method yazıldı
EmailAccountController içerisine create methodu yazıldı
EmailAccountController içerisine edit methodu yazıldı
EmailAccountController içerisine delete methodu yazıldı
Methodların Viewleri yazıldı EmailAccountController içerisinde hata düzeltmeleri yapıldı
ForwardingRule için ekleme güncelleme listeleme DTO ları yazıldı
ForwardingRuleService eklendi içine kullanıcının arayüzde yaptığı kural işlemlerini paketleyip ağ üzerinden api tarafındaki kontrollera gönderen method yazıldı
ForwardingRuleService içine formdan gelen kural verilerini apiye gönderen method yazıldı
ForwardingRuleService içine formdan gelen kural güncelleme verilerini apiye gönderen method yazıldı
ForwardingRuleService içine silinecek kuralı apiye dönderen method yazıldı
ForwardingRulesController eklendi içerisine listeleme methodu yazıldı
ForwardingRulesController içerisine ekleme methodu yazıldı
ForwardingRulesController içerisine düzenleme methodu yazıldı
ForwardingRulesController içerisine silme methodu yazıldı
ForwardingRulesController içerisindeki methodlar için Index, Create ve Edit viewleri eklendi ve yazıldı
Ortaya çıkan kural düzenledikten sonra kuralların gözükmemesi ve kural silme butonunun çalışmaması sorunları tespit edilip düzeltildi
PhoneNumber işlemleri için DTO ları oluşturuldu
PhoneNumberService eklendi
PhoneNumberService içerisine api tarafındaki methodları kullanmamızı sağlayacak kodlar yazıldı
PhoneNumbersConroller eklendi
PhoneNumbersController içerisine listeleme methodu yazıldı
PhoneNumbersController içerisine ekleme methodu yazıldı
PhoneNumbersController içerisine düzenleme methodu yazıldı
PhoneNumbersController içerisine silme methodu yazıldı
Silme butonundaki hata düzeltmeleri yapıldı
Projeti test etmek için Microsoft işyeri hesabı açıldı TentantId ClientId ClientSecret alındı ve uygulama arayüzünde başarıyla bağlandı daha sonra mail gönrerilerek denendi mesaj konsola yazdırıldı
Arayüzler profesyonelce yenilendi
Girilen mail sayısını kural sayısını ve şu ana kadar yollanan mesaj sayısını ana ekranda gösteren arayüz eklendi
Log kayıtlarını mesaj olarak ekrana vermek için arayüz veritabanında tutmak için model eklendi
Arayüzlerdeki ve api kısmındaki hatalar düzeltildi yeniden yazıldı


