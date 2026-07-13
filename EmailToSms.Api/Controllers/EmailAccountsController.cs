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
                EmailAddress = dto.EmailAddress,
                TenantId = dto.TentantId,
                ClientId = dto.ClientId,
                ClientSecret = dto.ClientSecret,
                LastChecketAt = DateTime.UtcNow,
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
                    EmailAddress = e.EmailAddress,
                    LastChecked = e.LastChecketAt

                })
                .ToListAsync();
            return Ok(accounts);
        }

        //E-posta güncelleme methodu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id , EmailAccountUpdateDto dto)
        {
            //kullanıcı Idsini çekiyoruz
            var userId = GetUserId();

            //sadece kullanıcıya ait E-Posta hesabı silinecek
            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId );

            if (account == null)
            {
                return NotFound(new { Message = "E-Posta bulunamadı veya yetkiniz yok" });
            }

            //hesabı güncelliyoruz
            account.EmailAddress = dto.EmailAddress;
            account.TenantId = dto.TentantId;
            account.ClientId = dto.ClientId;
            account.ClientSecret = dto.ClientSecret;

            //değişiklikleri veritabanına kaydediyoruz
            await _context.SaveChangesAsync();
            return Ok(new {Message = "E-Posta hesabı bilgileri başarıyla güncellendi"});
        }


        //E-posta silme meethodu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            //kullanıcı Idsini çekiyoruz
            var userId = GetUserId();

            //silinmek istenen hesabı buluyoruz sadece o kullanıcıya aitse silinecek
            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId );

            if (account == null) return NotFound(new { Message = "E-Posta bulunamadı veya yetkiniz yok" });

            //hesabı silip databaseimize kaydediyoruz
            _context.EmailAccounts.Remove(account);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "E-Posta hesabı başarıyla silindi" });

        }


    }
}
