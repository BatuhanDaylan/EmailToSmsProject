using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EmailToSms.Web.Controllers
{
    public class PhoneNumbersController : Controller
    {
        private readonly PhoneNumberService _phoneNumberService;
        public PhoneNumbersController(PhoneNumberService phoneNumberService)
        {
            _phoneNumberService = phoneNumberService;
        }

        //listeleme methodu
        [HttpGet]
        public async Task<IActionResult> Index(int ruleId , int accountId)
        {
            try
            {
                var numbers = await _phoneNumberService.GetNumberByRuleAsync(ruleId);

                //sayfada gezinirken ve geri dönerken kullanmak için veri saklıyoruz
                ViewBag.RuleId = ruleId;
                ViewBag.AccountId = accountId;

                //apiden çektiğimiz verileri viewe yolluyoruz
                return View(numbers);
            }
            catch(Exception ex)
            {
                return Content($"LİSTELEME HATASI: {ex.Message}");
            }
        }

        //kullanıcı ekleye tıkladığında çalışır
        [HttpGet]
        public IActionResult Create(int ruleId , int accountId)
        {
            var model = new PhoneNumberCreateViewModel { ForwardingRuleId = ruleId };
            ViewBag.AccountId = accountId;
            return View(model);
        }

        //kullanıcı veri girip kaydete bastığında çalışır
        [HttpPost]
        public async Task<IActionResult> Create(PhoneNumberCreateViewModel model , int accountId)
        {
            //doğrulandıysa AccountId yi geçici olarak kaydediyoruz
            if(!ModelState.IsValid)
            {
                ViewBag.AccountId = accountId;
                return View(model);
            }

            try
            {
                //viewmodeldeki verileri api tarafına yolluyoruz
                var isSuccess = await _phoneNumberService.AddPhoneNumberAsync(model);
                if(isSuccess) return RedirectToAction("Index", new { ruleId = model.ForwardingRuleId, accountId = accountId });

                ModelState.AddModelError(string.Empty, "Numara eklenirken API reddetti.");
                ViewBag.AccountId = accountId;
                return View(model);
            }
            catch(Exception ex)
            {
                return Content($"EKLEME HATASI: {ex.Message}");
            }
        }
        
        //kullanıcı edit sayfasına girdiğinde çalışır
        [HttpGet]
        public async Task<IActionResult> Edit(int id , int ruleId , int accountId)
        {
            try
            {
                //hesabın kurallarını çeker istediğimiz kuralı idye göre alırız
                var numbers = await _phoneNumberService.GetNumberByRuleAsync(ruleId);
                var number = numbers.FirstOrDefault(n => n.Id == id);

                if (number == null) return Content($"HATA: {id} ID'li numara bulunamadı!");

                //halihazırda var olan değiştirilecek numaranın değerlerini çekiyoruz
                var model = new PhoneNumberUpdateViewModel
                {
                    Id = number.Id,
                    ForwardingRuleId = ruleId,
                    PhoneNumber = number.PhoneNumber
                };

                //geçici kayıt
                ViewBag.AccountId = accountId;
                return View(model);
            }
            catch(Exception ex)
            {
                return Content($"DÜZENLEME EKRANI HATASI: {ex.Message}");
            }
        }

        //kullanıcı değişiklikleri yapıp kaydete bastığında
        [HttpPost]
        public async Task<IActionResult> Edit(int id , int accountId , PhoneNumberUpdateViewModel model)
        {
            //kullanıcının girdiği parametredeki id değeri ile urldeki aynımı
            if (id != model.Id) return Content("HATA: Güvenlik ihlali, ID'ler uyuşmuyor.");

            //kullanıcının girdiği verinin doğrulanması
            if (!ModelState.IsValid)
            {
                ViewBag.AccountId = accountId;
                //hata varsa kullanıcıya modeli geri yolla
                return View(model);
            }

            try
            {
                //güncellenmiş verileri ve hedefin id değerini apiye yolluyoruz
                var isSuccess = await _phoneNumberService.UpdatePhoneNumberAsync(id, model);
                //başarılı olurda kendi ait olduğu kuralın numara listesine yönlendiriyoruz
                if (isSuccess) return RedirectToAction("Index", new { ruleId = model.ForwardingRuleId, accountId = accountId });
                //başarısız olursa hata mesajıyla formu geri yolluyoruz
                ModelState.AddModelError(string.Empty , "Güncelleme reddedildi.");
                ViewBag.AccountId = accountId;
                return View(model);
            }
            catch(Exception ex)
            {
                return Content($"GÜNCELLEME HATASI: {ex.Message}");
            }
        }

        //silme methodu
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int ruleId, int accountId)
        {
            try
            {
                //Bir önceki aşamada öğrendiğimiz TempData mantığıyla güvenli silme
                await _phoneNumberService.DeletePhoneNumberAsync(id);
                return RedirectToAction("Index", new { ruleId = ruleId, accountId = accountId });
            }
            catch (Exception ex)
            {
                //Silme başarısız olursa siyah ekran yerine kendi sayfamızda kırmızı kutu gösteriyoruz
                TempData["ErrorMessage"] = $"API SİLME İŞLEMİNİ REDDETTİ: {ex.Message}";
                return RedirectToAction("Index", new { ruleId = ruleId, accountId = accountId });
            }
        }
    }
}
