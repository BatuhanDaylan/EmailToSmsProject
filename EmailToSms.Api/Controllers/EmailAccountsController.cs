using EmailToSms.Api.Common;
using EmailToSms.Api.Data;
using EmailToSms.Api.DTOs;
using EmailToSms.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    public class EmailAccountsController : CustomBaseController
    {
        private readonly EmailToSmsDbContext _context;
        private readonly IDataProtector _protector; // EKLENDİ

        // Constructor güncellendi
        public EmailAccountsController(EmailToSmsDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            // Şifreleme ve çözme işlemleri için benzersiz bir anahtar kelime belirliyoruz
            _protector = provider.CreateProtector("EmailToSms.MicrosoftGraph.Secret");
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] EmailAccountCreateDto dto)
        {
            // Kullanıcının ID'sini çekiyoruz
            var userId = GetUserId();

            // Bu kullanıcı bu maili daha önce eklemiş mi (Silinenler hariç)
            var isEmailExists = await _context.EmailAccounts
                .AnyAsync(e => e.UserId == userId && e.EmailAddress == dto.EmailAddress && !e.IsDeleted);

            if (isEmailExists)
            {
                return CreateActionResult(Result.Failure("Kayıt başarısız.", "Bu e-posta adresi zaten hesabınıza ekli.", 409));
            }

            // Girilen mail adresinin bilgilerini hazırlıyoruz
            var newAccount = new EmailAccount
            {
                UserId = userId,
                EmailAddress = dto.EmailAddress,
                TenantId = dto.TenantId,
                ClientId = dto.ClientId,
                // Kullanıcının girdiği şifreyi kriptolayarak kaydediyoruz
                ClientSecret = _protector.Protect(dto.ClientSecret),
                LastCheckedAt = DateTime.UtcNow
            };

            // Bilgileri veritabanımıza ekleyip kaydediyoruz
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
                    LastChecked = e.LastCheckedAt,
                    IsActive = e.IsActive
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
                return CreateActionResult(Result.Failure("Hesap bulunamadı.", "E-Posta bulunamadı veya yetkiniz yok.", 404));
            }

            //hesabı güncelliyoruz
            account.EmailAddress = dto.EmailAddress;
            account.TenantId = dto.TenantId;
            account.ClientId = dto.ClientId;
            // DÜZELTME: Güncelleme işleminde de yeni şifreyi kriptolayarak kaydediyoruz
            account.ClientSecret = _protector.Protect(dto.ClientSecret);

            //değişiklikleri veritabanına kaydediyoruz
            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("E-Posta hesabı bilgileri başarıyla güncellendi."));
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = GetUserId();
            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && !e.IsDeleted);

            if (account == null)
            {
                return CreateActionResult(Result.Failure("Hesap bulunamadı.", "E-Posta bulunamadı veya yetkiniz yok.", 404));
            }

            account.IsActive = !account.IsActive;
            await _context.SaveChangesAsync();

            var statusText = account.IsActive ? "aktif" : "pasif";
            return CreateActionResult(Result.Success($"E-posta hesabı başarıyla {statusText} duruma getirildi."));

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = GetUserId();

            var account = await _context.EmailAccounts
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (account == null)
            {
                return CreateActionResult(Result.Failure("Hesap bulunamadı.", "E-Posta bulunamadı veya yetkiniz yok.", 404));
            }

            // DÜZELTME: Veritabanından silmek yerine çöp kutusuna atıyoruz (Soft Delete)
            account.IsDeleted = true;
            account.IsActive = false; // Pasife de çekebiliriz

            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("E-Posta hesabı başarıyla silindi."));
        }
    }
}