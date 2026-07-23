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

        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber([FromBody] PhoneNumberCreateDto dto)
        {
            var userId = GetUserId();

            var rule = await _context.ForwardingRules
                .FirstOrDefaultAsync(r => r.Id == dto.ForwardingRuleId && r.EmailAccount!.UserId == userId);

            if (rule == null)
            {
                return CreateActionResult(Result.Failture("Yetkisiz işlem.", "Bu kural size ait değil veya bulunamadı.", 403));
            }

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return CreateActionResult(Result.Failture("Geçersiz Veri.", "Telefon numarası boş olamaz.", 400));
            }

            //telefon numarasının değerlerini atıyoruz
            var newPhoneNumber = new RulePhoneNumber
            {
                PhoneNumber = dto.PhoneNumber,
            };

            newPhoneNumber.ForwardingRules.Add(rule);

            _context.RulePhoneNumbers.Add(newPhoneNumber);
            await _context.SaveChangesAsync();

            return CreateActionResult(ResultOfT<int>.Success(newPhoneNumber.Id, "Telefon numarası başarıyla eklendi.", 201));
        }

        //numara listeleme methodu
        [HttpGet("rule/{ruleId}")]
        public async Task<IActionResult> GetNumbersByRule(int ruleId)
        {
            var userId = GetUserId();

            //ıdyi kontrol ediyoruz
            var ruleExist = await _context.ForwardingRules
                .AnyAsync(r => r.Id == ruleId && r.EmailAccount!.UserId == userId);

            if (!ruleExist)
            {
                return CreateActionResult(Result.Failture("Yetkisiz işlem.", "Bu kural size ait değil veya bulunamadı.", 403));
            }

            //numaraları listeliyoruz
            var numbers = await _context.RulePhoneNumbers
                .Where(p => p.ForwardingRules.Any(r => r.Id == ruleId))
                .Select(p => new PhoneNumberResponseDto
                {
                    Id = p.Id,
                    PhoneNumber = p.PhoneNumber,
                })
                .ToListAsync();

            return CreateActionResult(ResultOfT<List<PhoneNumberResponseDto>>.Success(numbers, "Numaralar listelendi."));
        }

        [HttpPut("{id}")] // HttpPost olarak yazılmıştı, standarda uyması için HttpPut'a çevrildi
        public async Task<IActionResult> UpdatePhoneNumber(int id, [FromBody] PhoneNumberUpdateDto dto)
        {
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return CreateActionResult(Result.Failture("Geçersiz Veri.", "Telefon numarası boş olamaz.", 400));
            }

            //Kuralın ve mail adresinin ıdsi kontrol ediliyor
            var phoneNumber = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRules)
                .ThenInclude(r => r.EmailAccount)
                .FirstOrDefaultAsync(p => p.Id == id && p.ForwardingRules.Any(r => r.EmailAccount!.UserId == userId));

            if (phoneNumber == null)
            {
                return CreateActionResult(Result.Failture("Bulunamadı.", "Telefon numarası bulunamadı veya yetkiniz yok.", 404));
            }

            //telefon numaramızı değiştiriyoruz
            phoneNumber.PhoneNumber = dto.PhoneNumber;

            //değiştirilen numara veri tabanında değiştirilir
            await _context.SaveChangesAsync();
            return CreateActionResult(Result.Success("Telefon numaranız başarıyla değiştirildi."));
        }

        //numara silme methodu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoneNumber(int id)
        {
            var userId = GetUserId();

            var phoneNumber = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRules)
                .ThenInclude(r => r.EmailAccount)
                .FirstOrDefaultAsync(p => p.Id == id && p.ForwardingRules.Any(r => r.EmailAccount!.UserId == userId));

            if (phoneNumber == null)
            {
                return CreateActionResult(Result.Failture("Bulunamadı.", "Telefon numarası bulunamadı veya yetkiniz yok.", 404));
            }

            // DÜZELTME: Soft Delete
            phoneNumber.IsDeleted = true;
            phoneNumber.IsActive = false;

            await _context.SaveChangesAsync();

            return CreateActionResult(Result.Success("Numara başarıyla silindi."));
        }
    }
}