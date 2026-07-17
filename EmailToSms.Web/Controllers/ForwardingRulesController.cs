using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}
