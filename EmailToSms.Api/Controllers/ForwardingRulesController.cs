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
    public class ForwardingRulesController : ControllerBase
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
                .AnyAsync(e => e.Id == dto.EmailAccountId && e.UserId==userId );

            //ıdye göre kontrol sağlıyoruz
            if (!emailAccountExists)
            {
                return Unauthorized(new { Message = "Seçilen E-Posta hesabı size ait değil veya bulunamadı" });
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

            return Ok(new {Message = "Kural ve başarıyla eklendi" , RuleId = newRule.Id });
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
                    SubjectKeyword= r.SubjectKeyword,
                    BodyKeyword= r.BodyKeyword
                })
                .ToListAsync();

            return Ok(rules);
        }

        //kural güncelleme methodu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRule(int id , [FromBody] ForwardingRuleUpdateDto dto)
        {
            var userId = GetUserId();

            //güncellenecek kuralın sahibini kontrol ediyoruz
            var rule = await _context.ForwardingRules
                .Include(r => r.EmailAccount)
                .FirstOrDefaultAsync(r => r.Id == id && r.EmailAccount!.UserId == userId);

            if (rule == null)
            {
                return NotFound(new { Message = "Kural bulunamadı veya yetkiniz yok" });
            }

            //kuralı güncelliyoruz
            rule.SenderKeyword = dto.SenderKeyword;
            rule.SubjectKeyword = dto.SubjectKeyword;
            rule.BodyKeyword = dto.BodyKeyword;

            //veritabanına kaydediyoruz
            await _context.SaveChangesAsync();
            return Ok(new {Message = "Kural başarıyla güncellendi"});
        }

        //kural silme methodu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var userId = GetUserId();

            //silincek kuralın sahibini kontrol ediyoruz
            var rule = await _context.ForwardingRules
                .Include (r => r.EmailAccount)
                .FirstOrDefaultAsync(r => r.Id == id && r.EmailAccount!.UserId == userId );

            if(rule == null)
            {
                return NotFound(new { Message = "Kural bulunamadı veya yetkiniz yok" });
            }

            //hesabı silip veritabanımıza kaydediyoruz
            _context.ForwardingRules .Remove(rule);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Kural ve bağlı tüm telefon numaraları başarıyla silindi" });
        }

    }
}
