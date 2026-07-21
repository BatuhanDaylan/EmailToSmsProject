using EmailToSms.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmailToSms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly EmailToSmsDbContext _context;

        public AuditLogsController(EmailToSmsDbContext context)
        {
            _context = context;
        }

        // GET: api/AuditLogs
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            // Logları veritabanından çekiyoruz (En yeni log en üstte olacak şekilde sıralıyoruz)
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(100) // Performans için son 100 logu sınır olarak belirleyebiliriz
                .ToListAsync();

            return Ok(logs);
        }
    }
}