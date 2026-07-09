using Microsoft.AspNetCore.Identity;

namespace EmailToSms.Api.Models
{
    //identitiy kullanıcısı için gerekli olan temel özellikleri içerir
    public class EmailToSmsUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
