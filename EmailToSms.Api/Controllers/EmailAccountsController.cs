using Microsoft.AspNetCore.Mvc;

namespace EmailToSms.Api.Controllers
{
    public class EmailAccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
