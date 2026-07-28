using EmailToSms.Web.Services;
using Microsoft.AspNetCore.Mvc;

public class LogsController : Controller
{
    // Artık HttpClient yerine doğrudan kendi servisimizi kullanıyoruz
    private readonly AuditLogService _auditLogService;

    public LogsController(AuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        // Servisten sayfalanmış veriyi çekiyoruz
        var paginatedData = await _auditLogService.GetPaginatedLogsAsync(page);

        // View'a gönderiyoruz
        return View(paginatedData);
    }
}