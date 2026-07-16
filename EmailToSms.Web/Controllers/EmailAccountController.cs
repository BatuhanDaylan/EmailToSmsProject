using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.ApplicationsWithAppId;
using Microsoft.Graph.Models;

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

            //apiye kayıt isteği atılır başarılıysa listeleme sayfasına döndürür
            var isSuccess = await _emailAccountService.AddAccountAsync(model);
            if (isSuccess) return RedirectToAction("index");

            //diğer durumlarda hata verir
            ModelState.AddModelError(string.Empty, "Hesap eklenirken bir hata oluştu.");
            return View(model);
        }

        //kullanıcının göreceği edit methodu
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //hesabı ıdye göre bul
            var accounts = await _emailAccountService.GetAccountsAsync();
            var account = accounts.FirstOrDefault(a => a.Id == id);

            if(account != null) return NotFound();

            var model = new EmailAccountUpdateViewModel
            {
                Id = account.Id,
                EmailAddress = account.EmailAddress
            };
            return View(model);
        }

        //kullanıcının apiye veri göndermesi için olan method
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmailAccountUpdateViewModel model)
        {
            //model doğrulanamamışsa
            if (ModelState.IsValid) return View(model);

            //apiye güncelleme isteği atılır başarılıysa listeleme sayfasına döndürür
            var isSuccess = await _emailAccountService.UpdateAccountAsync(id , model);
            if (isSuccess) return RedirectToAction("index");

            //diğer durumlarda hata verir
            ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
            return View(model);
        }


    }
}
