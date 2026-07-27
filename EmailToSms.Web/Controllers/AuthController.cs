using EmailToSms.Web.Services;
using EmailToSms.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailToSms.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        //kullanıcı tarayıcıya login adresi girdiğinde
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //kullanıcı giriş form verilerini yollarken
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //kurallar kontrol edilir uymuyorsa hatalarla birlikte yollanır
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //AuthService üzerinden apiye istek atıyoruz
            var isSuccess = await _authService.LoginAsync(model);

            //giriş başarılıysa yönlendiriyoruz
            if(isSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            //giriş başarılı değilse
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(model);
        }

        //kullanıcı tarayıcıya register adresini girdiğinde
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //kullanıcı kayıt form verilerini yollarken
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // kurallar kontrol edilir uymuyorsa hatalarla birlikte yollanır
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // DÜZELTME: Artık dönen sonucu ve mesajı birlikte alıyoruz
            var result = await _authService.RegisterAsync(model);

            if (result.IsSuccess)
            {
                // Kayıt başarılıysa kullanıcıyı doğrudan giriş yapması için Login sayfasına yönlendiriyoruz.
                return RedirectToAction("Login");
            }

            // DÜZELTME: Sabit mesaj yerine API'den gelen GERÇEK mesajı ekrana basıyoruz
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        //kullanıcı sistemden çıkış yaparken
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("login");
        }

    }
}
