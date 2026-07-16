using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailToSms.Web.Controllers
{
    public class EmailAccountController : Controller
    {
        private readonly EmailAccountService _emailAccountService;
        public EmailAccountController(EmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        //e-mail hesaplarını listeleme
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = await _emailAccountService.GetAccountsAsync();
            return View(accounts);

        }

        //kullanıcıya create gösterilirken
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //kullanıcıya veri girdirirken
        [HttpPost]
        public async Task<IActionResult> Create(EmailAccountCreateViewModel model)
        {
            //model doğrulanamamışsa
            if(!ModelState.IsValid) return View(model);

            var isSuccess = await _emailAccountService.AddAccountAsync(model);
            if (isSuccess) return RedirectToAction("index");

            ModelState.AddModelError(string.Empty, "Hesap eklenirken bir hata oluştu.");
            return View(model);
        }
    }
}
