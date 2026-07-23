using EmailToSms.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace EmailToSms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ResultOfT<T> result)
        {
            if (result.StatusCode == 204)
            {
                return new ObjectResult(null) { StatusCode = result.StatusCode };
            }

            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [NonAction]
        public IActionResult CreateActionResult(Result result)
        {
            if (result.StatusCode == 204)
            {
                return new ObjectResult(null) { StatusCode = result.StatusCode };
            }

            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }
    }
}