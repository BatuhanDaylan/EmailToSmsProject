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

    }
}
