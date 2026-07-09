namespace EmailToSms.Api.DTOs
{
    public class LoginDto
    {
        //ihtiyacımız olan rolleri alıyoruz
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
