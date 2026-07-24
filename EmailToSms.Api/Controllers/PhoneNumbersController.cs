using EmailToSms.Api.Data;
using EmailToSms.Api.DTOs;
using EmailToSms.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EmailToSms.Api.Common;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    public class PhoneNumbersController : CustomBaseController
    {
        private readonly EmailToSmsDbContext _context;
        public PhoneNumbersController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        //rehbere bağımsız numara ekle
        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber([FromBody] PhoneNumberCreateDto dto)
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return CreateActionResult(Result.Failure("Geçersiz Veri.", "Telefon numarası boş olamaz.", 400));

            var isPhoneExists = await _context.RulePhoneNumbers
                .AnyAsync(p => p.UserId == userId && p.PhoneNumber == dto.PhoneNumber && !p.IsDeleted);

            if (isPhoneExists)
                return CreateActionResult(Result.Failure("Çakışma.", "Bu numara rehberinizde zaten kayıtlı.", 409));

            //telefon numarasının değerlerini atıyoruz
            var newPhoneNumber = new RulePhoneNumber
            {
                UserId = userId,
                PhoneNumber = dto.PhoneNumber,
            };

            _context.RulePhoneNumbers.Add(newPhoneNumber);
            await _context.SaveChangesAsync();

            return CreateActionResult(ResultOfT<int>.Success(newPhoneNumber.Id, "Telefon numarası rehbere eklendi.", 201));
        }

        //kullanıcının tüm rehberini getir
        [HttpGet]
        public async Task<IActionResult> GetMyPhoneBook()
        {
            var userId = GetUserId();

            var numbers = await _context.RulePhoneNumbers
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => new PhoneNumberResponseDto
                {
                    Id = p.Id,
                    PhoneNumber = p.PhoneNumber,
                })
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<PhoneNumberResponseDto>>.Success(numbers, "Rehber listelendi."));
        }

        //kurala bağlı rehberi getir
        [HttpGet("rule/{ruleId}")]
        public async Task<IActionResult> GetNumbersByRule(int ruleId)
        {
            var userId = GetUserId();

            //ıdyi kontrol ediyoruz
            var ruleExist = await _context.ForwardingRules
                .AnyAsync(r => r.Id == ruleId && r.EmailAccount!.UserId == userId && !r.IsDeleted);

            if (!ruleExist)
                return CreateActionResult(Result.Failure("Yetkisiz işlem.", "Bu kural size ait değil veya bulunamadı.", 403));

            //numaraları listeliyoruz
            var numbers = await _context.RulePhoneNumbers
                .Where(p => p.ForwardingRules.Any(r => r.Id == ruleId) && !p.IsDeleted)
                .Select(p => new PhoneNumberResponseDto
                {
                    Id = p.Id,
                    PhoneNumber = p.PhoneNumber,
                })
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<PhoneNumberResponseDto>>.Success(numbers, "Kurala ait numaralar listelendi."));
        }

        //numarayı kurala bağla
        [HttpPost("assign")]
        public async Task<IActionResult> AssignToRule([FromBody] PhoneRuleAssignDto dto)
        {
            var userId = GetUserId();

            var phone = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRules)
                .FirstOrDefaultAsync(p => p.Id == dto.PhoneNumberId && p.UserId == userId);

            if (phone == null) return CreateActionResult(Result.Failure("Hata.", "Numara rehberinizde bulunamadı.", 404));

            var rule = await _context.ForwardingRules
                .FirstOrDefaultAsync(r => r.Id == dto.ForwardingRuleId && r.EmailAccount!.UserId == userId);

            if (rule == null) return CreateActionResult(Result.Failure("Hata.", "Kural bulunamadı.", 404));

            if (phone.ForwardingRules.Any(r => r.Id == rule.Id))
                return CreateActionResult(Result.Failure("Çakışma.", "Bu numara zaten bu kurala bağlı.", 409));

            phone.ForwardingRules.Add(rule);
            await _context.SaveChangesAsync();

            return CreateActionResult(Result.Success("Numara başarıyla kurala bağlandı."));
        }

        //numarayı kuraldan kopar
        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignFromRule([FromBody] PhoneRuleAssignDto dto)
        {
            var userId = GetUserId();

            var phone = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRules)
                .FirstOrDefaultAsync(p => p.Id == dto.PhoneNumberId && p.UserId == userId);

            if (phone == null) return CreateActionResult(Result.Failure("Hata.", "Numara bulunamadı.", 404));

            var rule = phone.ForwardingRules.FirstOrDefault(r => r.Id == dto.ForwardingRuleId);

            if (rule == null) return CreateActionResult(Result.Failure("Hata.", "Numara bu kurala zaten bağlı değil.", 404));

            phone.ForwardingRules.Remove(rule);
            await _context.SaveChangesAsync();

            return CreateActionResult(Result.Success("Numara ilgili kuraldan koparıldı."));
        }

        //numarayı güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoneNumber(int id, [FromBody] PhoneNumberUpdateDto dto)
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return CreateActionResult(Result.Failure("Geçersiz Veri.", "Telefon numarası boş olamaz.", 400));

            var phoneNumber = await _context.RulePhoneNumbers
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

            if (phoneNumber == null)
                return CreateActionResult(Result.Failure("Bulunamadı.", "Telefon numarası bulunamadı.", 404));

            phoneNumber.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("Telefon numaranız başarıyla değiştirildi."));
        }

        //rehberden tamamen sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoneNumber(int id)
        {
            var userId = GetUserId();

            var phoneNumber = await _context.RulePhoneNumbers
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (phoneNumber == null)
                return CreateActionResult(Result.Failure("Bulunamadı.", "Telefon numarası bulunamadı.", 404));

            phoneNumber.IsDeleted = true;
            phoneNumber.IsActive = false;

            await _context.SaveChangesAsync();

            return CreateActionResult(Result.Success("Numara rehberden tamamen silindi."));
        }
    }
}