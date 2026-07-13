using EmailToSms.Api.Models;

namespace EmailToSms.Api.Services
{
    public interface IMicrosoftGraphEmailReader
    {
        Task ReadEmailAndProcessAsync(EmailAccount account);
    }
}
