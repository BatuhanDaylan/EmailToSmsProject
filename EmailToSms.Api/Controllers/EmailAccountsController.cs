using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EmailToSms.Api.Models;
using EmailToSms.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using EmailToSms.Api.Common;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    public class EmailAccountsController : CustomBaseController
    {

        private readonly EmailToSmsDbContext _context;
        public EmailAccountsController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

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
                TenantId = dto.TenantId,
                ClientId = dto.ClientId,
                ClientSecret = dto.ClientSecret,
                LastCheckedAt = DateTime.UtcNow
            };

            //bilgileri databaseimize ekleyip kaydediyoruz
            _context.EmailAccounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return CreateActionResult(ResultOfT<int>.Success(newAccount.Id, "E-Posta hesabı başarıyla eklendi.", 201));
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
                    LastChecked = e.LastCheckedAt
                })
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<EmailAccountResponseDto>>.Success(accounts, "Hesaplar başarıyla listelendi."));
        }

        //E-posta güncelleme methodu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, EmailAccountUpdateDto dto)
        {
            //kullanıcı Idsini çekiyoruz
            var userId = GetUserId();

            //sadece kullanıcıya ait E-Posta hesabı silinecek
            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (account == null)
            {
                return CreateActionResult(Result.Failture("Hesap bulunamadı.", "E-Posta bulunamadı veya yetkiniz yok.", 404));
            }

            //hesabı güncelliyoruz
            account.EmailAddress = dto.EmailAddress;
            account.TenantId = dto.TenantId;
            account.ClientId = dto.ClientId;
            account.ClientSecret = dto.ClientSecret;

            //değişiklikleri veritabanına kaydediyoruz
            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("E-Posta hesabı bilgileri başarıyla güncellendi."));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = GetUserId();

            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (account == null)
            {
                return CreateActionResult(Result.Failture("Hesap bulunamadı.", "E-Posta bulunamadı veya yetkiniz yok.", 404));
            }

            // DÜZELTME: Veritabanından silmek yerine çöp kutusuna atıyoruz (Soft Delete)
            account.IsDeleted = true;
            account.IsActive = false; // Pasife de çekebiliriz

            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("E-Posta hesabı başarıyla silindi."));
        }
    }
}