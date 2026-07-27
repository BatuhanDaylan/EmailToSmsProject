using EmailToSms.Api.Common;
using EmailToSms.Api.DTOs;
using EmailToSms.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmailToSms.Api.Controllers
{
    public class AuthController : CustomBaseController
    {
        private readonly UserManager<EmailToSmsUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<EmailToSmsUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        //kullanıcı kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //nesnemizi oluşturuyoruz
            var user = new EmailToSmsUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            //şifreyi hashleyerek gönderir
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return CreateActionResult(Result.Success("Kullanıcı başarıyla kaydedildi.", 201));
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return CreateActionResult(Result.Failure("Kayıt işlemi başarısız.", errors, 400));
        }

        //giriş yapma işlemleri
        [HttpPost("login")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            //kullanıcı e posta adresine göre aranıyor
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return CreateActionResult(Result.Failure("Giriş başarısız.", "E-Posta veya şifre hatalı.", 401));

            //şifre kontrolü
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return CreateActionResult(Result.Failure("Giriş başarısız.", "E-Posta veya şifre hatalı.", 401));

            //şifre doğruysa
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //appsettings.json içerisindeki anahtarımızı şifreliyoruz
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            //tokenımızı oluşturacağız
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenResponse = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserId = user.Id
            };

            return CreateActionResult(ResultOfT<object>.Success(tokenResponse, "Başarıyla giriş yapıldı."));
        }
    }
}