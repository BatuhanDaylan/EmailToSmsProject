using EmailToSms.Web.Services;
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
    }
}
