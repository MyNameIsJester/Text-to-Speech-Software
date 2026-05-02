using Microsoft.AspNetCore.Mvc;
using AudioGuideAPI.Database;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class WebVisitsController : ControllerBase
{
    private readonly AppDbContext _db;

    public WebVisitsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] WebVisitRequest request)
    {
        var now = DateTime.UtcNow;

        var existing = await _db.WebVisits
            .FirstOrDefaultAsync(x => x.VisitToken == request.VisitToken);

        if (existing == null)
        {
            var visit = new WebVisit
            {
                VisitToken = request.VisitToken,
                FirstSeenAtUtc = now,
                LastSeenAtUtc = now,
                UserAgent = request.UserAgent,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _db.WebVisits.Add(visit);
        }
        else
        {
            existing.LastSeenAtUtc = now;
        }

        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat([FromBody] WebVisitRequest request)
    {
        var visit = await _db.WebVisits
            .FirstOrDefaultAsync(x => x.VisitToken == request.VisitToken);

        if (visit != null)
        {
            visit.LastSeenAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var total = await _db.WebVisits.CountAsync();

        var activeThreshold = DateTime.UtcNow.AddMinutes(-2);

        var active = await _db.WebVisits
            .CountAsync(x => x.LastSeenAtUtc >= activeThreshold);

        return Ok(new
        {
            totalVisits = total,
            activeUsers = active
        });
    }
}

public class WebVisitRequest
{
    public string VisitToken { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
}