using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index()
        {
            try
            {
                var numbers = await _phoneNumberService.GetMyPhoneBookAsync();
                //apiden çektiğimiz verileri viewe yolluyoruz
                return View(numbers);
            }
            catch (Exception ex)
            {
                return Content($"REHBER LİSTELEME HATASI: {ex.Message}");
            }
        }

        //kullanıcı ekleye tıkladığında çalışır
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //kullanıcı veri girip kaydete bastığında çalışır
        [HttpPost]
        public async Task<IActionResult> Create(PhoneNumberCreateViewModel model)
        {
            //doğrulandıysa AccountId yi geçici olarak kaydediyoruz

            try
            {
                //viewmodeldeki verileri api tarafına yolluyoruz
                var isSuccess = await _phoneNumberService.AddPhoneNumberAsync(model);
                if (isSuccess) return RedirectToAction("Index");

                ModelState.AddModelError(string.Empty, "Numara eklenirken bir hata oluştu (Zaten rehberinizde olabilir).");
                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"EKLEME HATASI: {ex.Message}");
            }
        }

        //kullanıcı edit sayfasına girdiğinde çalışır
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var numbers = await _phoneNumberService.GetMyPhoneBookAsync();
                var number = numbers.FirstOrDefault(n => n.Id == id);
                if (number == null) return Content("HATA: Numara bulunamadı!");

                //halihazırda var olan değiştirilecek numaranın değerlerini çekiyoruz
                var model = new PhoneNumberUpdateViewModel
                {
                    Id = number.Id,
                    PhoneNumber = number.PhoneNumber
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"DÜZENLEME EKRANI HATASI: {ex.Message}");
            }
        }

        //kullanıcı değişiklikleri yapıp kaydete bastığında
        [HttpPost]
        public async Task<IActionResult> Edit(int id, PhoneNumberUpdateViewModel model)
        {
            if (id != model.Id) return Content("HATA: Güvenlik ihlali.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _phoneNumberService.UpdatePhoneNumberAsync(id, model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                //hata varsa kullanıcıya modeli geri yolla
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _phoneNumberService.DeletePhoneNumberAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Silme Hatası: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        //kural rehber ilişkisi
        [HttpGet]
        public async Task<IActionResult> RuleNumbers(int ruleId, int accountId)
        {
            try
            {
                var attachedNumbers = await _phoneNumberService.GetNumbersByRuleAsync(ruleId);
                var allNumbers = await _phoneNumberService.GetMyPhoneBookAsync();

                ViewBag.RuleId = ruleId;
                ViewBag.AccountId = accountId;
                ViewBag.AllNumbers = allNumbers;

                return View(attachedNumbers);
            }
            catch (Exception ex)
            {
                return Content($"KURAL NUMARALARI HATASI: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int ruleId, int accountId, int phoneNumberId)
        {
            try
            {
                var model = new PhoneRuleAssignViewModel { ForwardingRuleId = ruleId, PhoneNumberId = phoneNumberId };
                await _phoneNumberService.AssignToRuleAsync(model);
                return RedirectToAction("RuleNumbers", new { ruleId, accountId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Bağlama işlemi başarısız oldu (Numara zaten bu kurala bağlı olabilir).";
                return RedirectToAction("RuleNumbers", new { ruleId, accountId });
            }
        }

        //silme methodu
        [HttpPost]
        public async Task<IActionResult> Unassign(int ruleId, int accountId, int phoneNumberId)
        {
            try
            {
                var model = new PhoneRuleAssignViewModel { ForwardingRuleId = ruleId, PhoneNumberId = phoneNumberId };
                await _phoneNumberService.UnassignFromRuleAsync(model);
                return RedirectToAction("RuleNumbers", new { ruleId, accountId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Koparma işlemi başarısız oldu.";
                return RedirectToAction("RuleNumbers", new { ruleId, accountId });
            }
        }
    }
}