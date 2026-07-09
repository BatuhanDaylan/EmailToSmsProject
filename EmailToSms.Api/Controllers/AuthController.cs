using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmailToSms.Api.Models;
using EmailToSms.Api.DTOs;
using Microsoft.Graph.Models;
using System.Linq.Expressions;

namespace EmailToSms.Api.Controllers
{
    //dışarıdan bu sınıfa nasıl ulaşılacağı
    [Route("api/[controller]")]
    //ModelState.IsValid otomatik olarak yapılır ve sınıfın bir veri döndüreceğini söyler
    [ApiController]
    public class AuthController : ControllerBase
    {
        //veritabanı işlemlerinin yapılmasını sağlar
        private readonly UserManager<EmailToSmsUser> _userManager;
        //JWT tokenın okunmasını sağlar
        private readonly IConfiguration _configuration;

        //projemizin içine entegre ediyoruz
        public AuthController(UserManager<EmailToSmsUser> userManager , IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
     
        // kayıt yapma işlemleri
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

            if(result.Succeeded)
            {
                return Ok(new {Message = "Kullanıcı kaydedildi"});
            }

            return BadRequest(result.Errors);
        }

        //giriş yapma işlemleri
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            //kullanıcı e posta adresine göre aranıyor
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new { Message = "E-Posta veya şifte hatalı" });

            //şifre kontrolü
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid) return Unauthorized(new { Message = "E-Posta veya şifre hatalı" });

            //şifre doğruysa
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName!),
                //Id veritabanıyla olan bağımız
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                //JWT için benzersiz tanımlayıcı talebi oluşturmak için kullanılır
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
            };

            //appsettings.json içerisindeki anahtarımızı şifreliyoruz
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            //tokenımızı oluşturacağız
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(3),
                claims: authClaims,
                //token anahtarımıza mühürleniyor
                signingCredentials: new SigningCredentials(authSigninKey , SecurityAlgorithms.HmacSha256)
                );

            //tokenı string metne çevireceğiz
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserId = user.Id
            });
        }

    }
}
