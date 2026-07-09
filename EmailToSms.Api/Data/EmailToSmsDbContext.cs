using Microsoft.EntityFrameworkCore;
using EmailToSms.Api.Models;

namespace EmailToSms.Api.Data
{
    //Sadece uygulamanın asıl iş modelini tutan veritabanı beyni
    public class EmailToSmsDbContext : DbContext
    {
        public EmailToSmsDbContext(DbContextOptions<EmailToSmsDbContext> options) : base(options)
        {
        }

        //Uygulama tabloları
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<ForwardingRule> ForwardingRules { get; set; }
        public DbSet<RulePhoneNumber> RulePhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Tablolar arası ilişkileri garanti altına alıyoruz

            //E-posta hesabı silinirse ona bağlı kurallar da otomatik silinsin (Cascade Delete)
            modelBuilder.Entity<ForwardingRule>()
                .HasOne(r => r.EmailAccount)
                .WithMany(e => e.Rules)
                .HasForeignKey(r => r.EmailAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            //Bir kural silinirse ona bağlı telefon numaraları da otomatik silinsin
            modelBuilder.Entity<RulePhoneNumber>()
                .HasOne(p => p.ForwardingRule)
                .WithMany(r => r.TargetPhoneNumbers)
                .HasForeignKey(p => p.ForwardingRuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}