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
    public class ForwardingRulesController : CustomBaseController
    {
        //Veritabanımızı enjekte ediyoruz
        private readonly EmailToSmsDbContext _context;
        public ForwardingRulesController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        //token içerisinden id değerini alıyoruz
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        //yeni kural ekleme methodu
        [HttpPost]
        public async Task<IActionResult> AddRule([FromBody] ForwardingRuleCreateDto dto)
        {
            var userId = GetUserId();

            //ıdyi kontrol ediyoruz
            var emailAccountExists = await _context.EmailAccounts
                .AnyAsync(e => e.Id == dto.EmailAccountId && e.UserId == userId);

            if (!emailAccountExists)
            {
                return CreateActionResult(Result.Failure("Yetkisiz işlem.", "Seçilen E-Posta hesabı size ait değil veya bulunamadı.", 403));
            }

            //Birebir aynı kural bu e-posta hesabı için zaten var mı
            var isRuleExists = await _context.ForwardingRules
                .AnyAsync(r => r.EmailAccountId == dto.EmailAccountId
                            && r.SenderKeyword == dto.SenderKeyword
                            && r.SubjectKeyword == dto.SubjectKeyword
                            && r.BodyKeyword == dto.BodyKeyword
                            && !r.IsDeleted);

            if (isRuleExists)
            {
                return CreateActionResult(Result.Failure("Kayıt başarısız.", "Tamamen aynı filtrelere sahip bir kural zaten mevcut.", 409));
            }

            //kuralları eklediğimi yapı
            var newRule = new ForwardingRule
            {
                EmailAccountId = dto.EmailAccountId,
                SenderKeyword = dto.SenderKeyword,
                SubjectKeyword = dto.SubjectKeyword,
                BodyKeyword = dto.BodyKeyword
            };

            //veritabanımıza ekleyip kaydediyoruz
            _context.ForwardingRules.Add(newRule);
            await _context.SaveChangesAsync();

            return CreateActionResult(ResultOfT<int>.Success(newRule.Id, "Kural başarıyla eklendi.", 201));
        }

        //kuralları listeleme methodu
        [HttpGet("email/{emailAccountId}")]
        public async Task<IActionResult> GetRulesByEmailAccount(int emailAccountId)
        {
            var userId = GetUserId();

            //sadece bu kullanıcıya ait mail hesabının kurallarını ve telefon numaralarını getirir
            var rules = await _context.ForwardingRules
                .Where(r => r.EmailAccountId == emailAccountId && r.EmailAccount!.UserId == userId)
                .Select(r => new ForwardingRuleResponseDto
                {
                    Id = r.Id,
                    SenderKeyword = r.SenderKeyword,
                    SubjectKeyword = r.SubjectKeyword,
                    BodyKeyword = r.BodyKeyword
                })
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<ForwardingRuleResponseDto>>.Success(rules, "Kurallar başarıyla listelendi."));
        }

        //kural güncelleme methodu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRule(int id, [FromBody] ForwardingRuleUpdateDto dto)
        {
            var userId = GetUserId();

            //güncellenecek kuralın sahibini kontrol ediyoruz
            var rule = await _context.ForwardingRules
                .Include(r => r.EmailAccount)
                .FirstOrDefaultAsync(r => r.Id == id && r.EmailAccount!.UserId == userId);

            if (rule == null)
            {
                return CreateActionResult(Result.Failure("Bulunamadı.", "Kural bulunamadı veya yetkiniz yok.", 404));
            }

            //Güncellenmek istenen veriler başka bir kuralla aynı mı oluyor
            var isDuplicateRuleExists = await _context.ForwardingRules
                .AnyAsync(r => r.EmailAccountId == rule.EmailAccountId
                            && r.Id != id //Kendisi hariç
                            && r.SenderKeyword == dto.SenderKeyword
                            && r.SubjectKeyword == dto.SubjectKeyword
                            && r.BodyKeyword == dto.BodyKeyword
                            && !r.IsDeleted);

            if (isDuplicateRuleExists)
            {
                return CreateActionResult(Result.Failure("Güncelleme başarısız.", "Tamamen aynı filtrelere sahip başka bir kuralınız zaten mevcut.", 409));
            }

            //kuralı güncelliyoruz
            rule.SenderKeyword = dto.SenderKeyword;
            rule.SubjectKeyword = dto.SubjectKeyword;
            rule.BodyKeyword = dto.BodyKeyword;

            //veritabanına kaydediyoruz
            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("Kural başarıyla güncellendi."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var userId = GetUserId();

            var rule = await _context.ForwardingRules
                .Include(r => r.EmailAccount)
                .FirstOrDefaultAsync(r => r.Id == id && r.EmailAccount!.UserId == userId);

            if (rule == null)
            {
                return CreateActionResult(Result.Failure("Bulunamadı.", "Kural bulunamadı veya yetkiniz yok.", 404));
            }

            // DÜZELTME: Soft Delete
            rule.IsDeleted = true;
            rule.IsActive = false;

            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("Kural başarıyla silindi."));
        }
    }
}