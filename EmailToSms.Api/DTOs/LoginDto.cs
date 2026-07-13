using Microsoft.Graph.Models;
using System.ComponentModel.DataAnnotations;

namespace EmailToSms.Api.DTOs
{
    public class LoginDto
    {
        //ihtiyacımız olan rolleri alıyoruz
        [Required , EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
