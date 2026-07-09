using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EmailToSms.Api.Models;
using EmailToSms.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph.Models;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class EmailAccountsController : Controller
    {

        //uygulama veritabanımızı enjekte ediyotuz
        private readonly EmailToSmsDbContext _context;
        public EmailAccountsController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        //token içinden Id değerlerini alır
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] EmailAccountCreateDto dto) 
        {

            //kullanıcının ıdsini çekiyoruz
            var userId = GetUserId();

            //girilen mail adresinin bilgilerini kaydediyoruz
            var newAccount = new EmailAccount
            {
                UserId = userId,
                EmailAddress = dto.EmailAdress,
                TenantId = dto.TentantId,
                ClientId = dto.ClientId,
                ClientSecret = dto.ClientSecret,
            };

            //bilgileri databaseimize ekleyip kaydediyoruz
            _context.EmailAccounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "E-posta hesabı başarıyla eklendi", AccountId = newAccount.Id});
        }


    }
}
