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
    public class EmailAccountsController : ControllerBase
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

        //E-posta kayıt methodu
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

            return Ok(new { Message = "E-Posta hesabı başarıyla eklendi", AccountId = newAccount.Id});
        }

        //E-posta listeleme methodu
        [HttpGet]
        public async Task<IActionResult> GetMyAccounts()
        {

            //kullanıcı Idsini çekiyoruz
            var userId = GetUserId();

            //girilmiş olan mail bilgilerini liste haline getirip çekiyoruz
            var accounts = await _context.EmailAccounts
                .Where(e => e.UserId == userId)
                .Select(e => new EmailAccountResponseDto
                {
                    Id = e.Id,
                    EmailAdress = e.EmailAddress,
                    LastChecked = e.LastChecketAt

                })
                .ToListAsync();
            return Ok(accounts);
        }

        //E-posta silme meethodu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            //kullanıcı Idsini çekiyoruz
            var userId = GetUserId();

            //silinmek istenen hesabı buluyoruz sadece o kullanıcıya aitse silinecek
            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId==userId);

            if (account == null) return NotFound(new { Message = "E-Posta bulunamadı veya yetkiniz yok" });
            _context.EmailAccounts.Remove(account);

            //hesabı silip databaseimize kaydediyoruz
            await _context.SaveChangesAsync();
            return Ok(new { Message = "E-Posta hesabı başarıyla silindi" });

        }


    }
}
