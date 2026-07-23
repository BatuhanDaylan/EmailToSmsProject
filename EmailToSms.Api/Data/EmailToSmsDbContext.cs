using Microsoft.EntityFrameworkCore;
using EmailToSms.Api.Models;

namespace EmailToSms.Api.Data
{
    public class EmailToSmsDbContext : DbContext
    {
        public EmailToSmsDbContext(DbContextOptions<EmailToSmsDbContext> options) : base(options)
        {
        }

        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<ForwardingRule> ForwardingRules { get; set; }
        public DbSet<RulePhoneNumber> RulePhoneNumbers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Çoka Çok (N:N) İlişki Ara Tablosu (Junction Table)
            modelBuilder.Entity<ForwardingRule>()
                .HasMany(r => r.TargetPhoneNumbers)
                .WithMany(p => p.ForwardingRules)
                .UsingEntity(j => j.ToTable("ForwardingRulePhoneNumbers"));

            //Global Query Filters (Sadece silinmemiş kayıtları getir)
            modelBuilder.Entity<EmailAccount>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ForwardingRule>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<RulePhoneNumber>().HasQueryFilter(p => !p.IsDeleted);

            //Partial Unique Indexes (PostgreSQL için Npgsql formatı)
            //Bir kullanıcının aynı e-posta adresini iki kez eklemesini engeller (Silinmemişse)
            modelBuilder.Entity<EmailAccount>()
                .HasIndex(e => new { e.UserId, e.EmailAddress })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            //Bir telefon numarasının sisteme bir kez eklenmesini sağlar (Silinmemişse)
            modelBuilder.Entity<RulePhoneNumber>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            //Tablolar arası 1:N İlişkiler (Cascade işlemleri)
            modelBuilder.Entity<ForwardingRule>()
                .HasOne(r => r.EmailAccount)
                .WithMany(e => e.Rules)
                .HasForeignKey(r => r.EmailAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne<ForwardingRule>()
                .WithMany()
                .HasForeignKey(a => a.ForwardingRuleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}