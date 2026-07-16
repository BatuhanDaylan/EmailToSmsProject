using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmailToSms.Web.Controllers
{
    public class EmailAccountsController : Controller
    {
        private readonly EmailAccountService _emailAccountService;

        public EmailAccountsController(EmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        //tarayıcıdan sayfaya girildiğinde çalışır
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                //kayıtlı postaları çekiyoruz
                var accounts = await _emailAccountService.GetAccountsAsync();
                return View(accounts);
            }
            catch (Exception ex)
            {
                //Eğer API'ye bağlanamazsa hatayı ekrana basacak
                return Content($"API BAĞLANTI HATASI: {ex.Message}");
            }
        }

        //ekle butonuna basıldığında çalışır
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //kullanıcı veri ekleyip gönderirken savaşır
        [HttpPost]
        public async Task<IActionResult> Create(EmailAccountCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                //kullanıcının forma doldurduğu verileri apiye kaydetmesini söylüyoruz
                var isSuccess = await _emailAccountService.AddAccountAsync(model);
                if (isSuccess) return RedirectToAction("Index");

                //api redderse
                ModelState.AddModelError(string.Empty, "Hesap eklenirken API reddetti.");
                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"EKLEME HATASI: {ex.Message}");
            }
        }

        //düzenle sayfasına girildiğinde
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                //Idye göre verileri getiriyoruz
                var accounts = await _emailAccountService.GetAccountsAsync();
                var account = accounts.FirstOrDefault(a => a.Id == id);

                if (account == null)
                {
                    return Content($"HATA: {id} ID'li hesap bulunamadı! (API boş liste döndürmüş veya oturumunuz düşmüş olabilir.)");
                }

                var model = new EmailAccountUpdateViewModel
                {
                    Id = account.Id,
                    EmailAddress = account.EmailAddress
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"DÜZENLEME EKRANI HATASI: {ex.Message}");
            }
        }

        //veri düzenlendikten sonra yollanırken
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmailAccountUpdateViewModel model)
        {
            if (id != model.Id) return Content("HATA: Güvenlik ihlali, ID'ler uyuşmuyor.");
            if (!ModelState.IsValid) return View(model);

            try
            {
                var isSuccess = await _emailAccountService.UpdateAccountAsync(id, model);
                if (isSuccess) return RedirectToAction("Index");

                ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                return View(model);
            }
            catch (Exception ex)
            {
                return Content($"GÜNCELLEME KAYIT HATASI: {ex.Message}");
            }
        }

        //hesap silme işlemi
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _emailAccountService.DeleteAccountAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content($"SİLME HATASI: {ex.Message}");
            }
        }
    }
}