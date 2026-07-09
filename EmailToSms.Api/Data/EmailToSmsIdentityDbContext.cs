using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmailToSms.Api.Models;

namespace EmailToSms.Api.Data
{
    public class EmailToSmsIdentityDbContext : IdentityDbContext<EmailToSmsUser>
    {
        public EmailToSmsIdentityDbContext(DbContextOptions<EmailToSmsIdentityDbContext> options) : base(options)
        {
        }
    }
}
