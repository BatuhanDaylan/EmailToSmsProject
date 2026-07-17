using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace EmailToSms.Web.Controllers
{
    public class ForwardingRulesController : Controller
    {
        private readonly ForwardingRuleService _ruleService;
        public ForwardingRulesController(ForwardingRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        //kullanıcı listeleme sayfasına tıkladığında
        [HttpGet]
        public async Task<IActionResult> Index(int accountId)
        {
            try
            {
                var rules = await _ruleService.GetRulesByAccountIdAsync(accountId);
                ViewBag.EmailAccountId = accountId;
                return Ok(rules);
            }
            catch (Exception ex)
            {
                return Content($"KURAL LİSTELEME HATASI: {ex.Message}");
            }
        }

        //kullanıcı ekle sayfasına tıkladığında
        [HttpGet]
        public IActionResult Create(int accountId)
        {
            var model = new ForwardingRuleCreateViewModel
            {
                EmailAccountId = accountId
            };
            return View(model);
        }

        //kullanıcı veri kaydederken
        [HttpPost]
        public async Task<IActionResult> Create(ForwardingRuleCreateViewModel model)
        {
            //eğer doğrulanmamışsa modele yolla
            if(!ModelState.IsValid) return View(model);

            try
            {   //kullanıcının forma doldurduğu verileri apiye kaydetmesini söylüyoruz
                var isSuccess = await _ruleService.AddRuleAsync(model);
                if (isSuccess) return RedirectToAction("index", new { accountId = model.EmailAccountId });
                ModelState.AddModelError(string.Empty , "Kural eklenirken APİ reddetti.");
                return View(model);
            }
            catch(Exception ex)
            {
                return Content($"KURAL EKLEME HATASI: {ex.Message}");
            }
        }

        //kural düzenleme sayfasına girildiğinde
        [HttpGet]
        public async Task<IActionResult> Edit(int id , int  accountId)
        {
            try
            {
                //hesabın kurallarını çekeriz kurallar arasından Id sine göre çekeriz
                var rules = await _ruleService.GetRulesByAccountIdAsync(accountId);
                var rule = rules.FirstOrDefault(r => r.Id == id);

                if (rule == null) return Content($"HATA: {id} ID'li kural bulunamadı!");

                //kuralın elemanlarını çekiyoruz
                var model = new ForwardingRuleUpdateViewModel
                {
                    Id = rule.Id,
                    EmailAccountId = rule.EmailAccountId,
                    SenderKeyword = rule.SenderKeyword,
                    SubjectKeyword = rule.SubjectKeyword,
                    BodyKeyword = rule.BodyKeyword,
                };
                return View(model);
            }
            catch(Exception ex)
            {
                return Content($"DÜZENLEME EKRANI HATASI: {ex.Message}");
            }
        }

        //kullanıcı düzenlenen verileri gönderirken
        [HttpPost]
        public async Task<IActionResult> Edit(int id , ForwardingRuleUpdateViewModel model)
        {
            //idler kontrol edilir doğrulama kontrol edilir
            if (id != model.Id) return Content("HATA: Güvenlik ihlali, ID'ler uyuşmuyor.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                //method çağırılır Id si ve yeni verileri servise iletilir
                var isSuccess = await _ruleService.UpdateRuleAsync(id, model);
                if (isSuccess) return RedirectToAction("Index" , new { accountId = model.EmailAccountId });

                ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"GÜNCELLEME KAYIT HATASI: {ex.Message}");
            }
        }

        //kullanıcı kural silerken
        [HttpDelete]
        public async Task<IActionResult> Delete(int id , int accountId)
        {
            try
            {
                await _ruleService.DeleteRuleAsync(id);
                return RedirectToAction("Index", new { accountId = accountId });
            }
            catch (Exception ex)
            {
                return Content($"SİLME HATASI: {ex.Message}");
            }
        }
    }
}
