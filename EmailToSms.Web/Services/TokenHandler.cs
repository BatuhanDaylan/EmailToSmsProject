using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EmailToSms.Web.Services
{
    //http istekleri arasına giren bir şifre
    public class TokenHandler : DelegatingHandler
    {
        //kullanıcının o anki oturumunu istemek için dependency injection
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //herhangi bir api isteği yola çıkmadan çalışan ana method
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //kullanıcının mevcut web tarayıcısı bağlamını alıyoruz
            var httpContext = _httpContextAccessor.HttpContext;
            //tokenımızı okuyoruz
            var token = httpContext?.Session.GetString("ApiJwt");
            //token boş değilse
            if (!string.IsNullOrWhiteSpace(token))
            {
                //apiye giden isteğin başlık kısmına yetkilendirme
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}