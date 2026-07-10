using EmailToSms.Api.Data;
using EmailToSms.Api.DTOs;
using EmailToSms.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System.Security.Claims;

namespace EmailToSms.Api.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class PhoneNumbersController : Controller
    {
        private readonly EmailToSmsDbContext _context;
        public PhoneNumbersController(EmailToSmsDbContext context)
        {
            _context = context;
        }
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        //numara ekleme methodu
        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber([FromBody] PhoneNumberCreateDto dto)
        {
            var userId = GetUserId();

            //ıdyi kontrol ediyoruz
            var ruleExist = await _context.ForwardingRules
                .AnyAsync(r => r.Id == dto.ForwardingRuleId && r.EmailAccount!.UserId == userId );

            if(!ruleExist)
            {
                return Unauthorized(new { Message = "Bu kural size ait değil veya bulunamadı" });
            }

            if(string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return BadRequest(new { Message = "Telefon numarası boş olamaz" });
            }

            //telefon numarasının değerlerini atıyoruz
            var newPhoneNumber = new RulePhoneNumber
            {
                ForwardingRuleId = dto.ForwardingRuleId,
                PhoneNumber = dto.PhoneNumber,
            };

            //telefon numaramızı veritabanına ekleyip kaydediyoruz
            _context.RulePhoneNumbers.Add(newPhoneNumber);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Telefon numarası başarıyla eklendi" , RulePhoneNumberId = newPhoneNumber.Id });
        }

        //numara listeleme methodu
        [HttpGet("rule/{ruleId}")]
        public async Task<IActionResult> GetNumbersByRule(int ruleId)
        {
            var userId = GetUserId();

            //ıdyi kontrol ediyoruz
            var ruleExist = await _context.ForwardingRules
                .AnyAsync(r => r.Id == ruleId && r.EmailAccount!.UserId == userId);

            if(!ruleExist)
            {
                return Unauthorized(new { Message = "Bu kural size ait değil veya bulunamadı" });
            }

            //numaraları listeliyoruz
            var numbers = await _context.RulePhoneNumbers
                .Where(p => p.ForwardingRuleId == ruleId)
                .Select(p => new PhoneNumberResponseDto
                {
                    Id = p.Id,
                    PhoneNumber = p.PhoneNumber,
                })
                .ToListAsync();

            //listelediğimiz numaralarımızı döndürüyoruz
            return Ok(numbers);
        }

        //numara güncelleme methodu
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdatePhoneNumber(int id , [FromBody] PhoneNumberUpdateDto dto)
        {
            var userId = GetUserId();

            if(string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return BadRequest(new { Message = "Telefon numarası boş olamaz" });
            }

            //Kuralın ve mail adresinin ıdsi kontrol ediliyor
            var phoneNumber = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRule)
                .ThenInclude(r => r!.EmailAccount)
                .FirstOrDefaultAsync(p => p.Id == id && p.ForwardingRule!.EmailAccount.UserId == userId);

            if(phoneNumber == null)
            {
                return NotFound(new { Message = "Telefon numarası bulunamadı yada yetkiniz yok" });
            }

            //telefon numaramızı değiştiriyoruz
            phoneNumber.PhoneNumber = dto.PhoneNumber;

            //değiştirilen numara veri tabanında değiştirilir
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Telefon numaranız başarıyla değiştirildi" });
        }

        //numara silme methodu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoneNumber(int id)
        {
            var userId = GetUserId();

            //Kuralın ve mail adresinin ıdsi kontrol ediliyor
            var phoneNumber = await _context.RulePhoneNumbers
                .Include(p => p.ForwardingRule)
                .ThenInclude(r => r!.EmailAccount)
                .FirstOrDefaultAsync(p => p.Id == id && p.ForwardingRule!.EmailAccount.UserId == userId);

            if(phoneNumber == null)
            {
                return NotFound(new { Message = "Telefon numarası bulunamadı yada yetkiniz yok" });
            }

            //telefon numarasını veritabanından silip kaydediyoruz
            _context.RulePhoneNumbers.Remove(phoneNumber);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Numara başarıyla silindi" });
        }
    }
}
