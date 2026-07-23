namespace EmailToSms.Api.Models
{
    //diğer sınıflarımızın miras alacağı abstract sınıf
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = false;
    }
}
