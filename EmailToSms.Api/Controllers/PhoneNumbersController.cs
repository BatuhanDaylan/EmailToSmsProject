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
    }
}
