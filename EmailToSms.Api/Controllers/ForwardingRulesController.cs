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
    public class ForwardingRulesController : Controller
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

        //yeni kural ekleme fonksiyonu
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

            var newRule = new ForwardingRule
            {
                EmailAccountId = dto.EmailAccountId,
                SenderKeyword = dto.SenderKeyword,
                SubjectKeyword = dto.SubjectKeyword,
                BodyKeyword = dto.BodyKeyword

            };

            foreach(var phone in dto.PhoneNumbers)
            {
                if(!string.IsNullOrWhiteSpace(phone))
                {
                    newRule.TargetPhoneNumbers.Add(new RulePhoneNumber { PhoneNumber = phone });
                }
            }

            _context.ForwardingRules.Add(newRule);
            await _context.SaveChangesAsync();



            return Ok(new {Message = "Kural ve telefon numaraları başarıyla eklendi" , RuleId = newRule.Id });
        }

    }
}
