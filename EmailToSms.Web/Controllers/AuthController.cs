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
        //kullanıcı tarayıcıya adresi girdiğinde
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //kullanıcı form verilerini yollarken
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //kurallar kontrol edilir uymuyorsa hatalarla birlikte yollanır
            if (ModelState.IsValid)
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
    }
}
