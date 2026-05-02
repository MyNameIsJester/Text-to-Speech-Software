using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    public class StartPlaybackRequestDto
    {
        public int FoodStallId { get; set; }
        public string LanguageCode { get; set; } = "";
        public string TriggerType { get; set; } = "";
    }

    public class StartPlaybackResponseDto
    {
        public int PlaybackLogId { get; set; }
        public string Message { get; set; } = "";
    }

    public class FinishPlaybackRequestDto
    {
        public int PlaybackLogId { get; set; }
        public string Status { get; set; } = "";
        public int ActualDurationSeconds { get; set; }
    }

    public class PlaybackLogAdminDto
    {
        public int Id { get; set; }
        public int FoodStallId { get; set; }
        public string FoodStallAddress { get; set; } = "";
        public string VietnameseName { get; set; } = "";
        public string EnglishName { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public string TriggerType { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int ActualDurationSeconds { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PlaybackLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaybackLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartPlayback([FromBody] StartPlaybackRequestDto request)
        {
            if (request == null || request.FoodStallId <= 0)
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ.");
            }

            var foodStallExists = await _context.FoodStalls
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.FoodStallId);

            if (!foodStallExists)
            {
                return NotFound("Food stall not found.");
            }

            var log = new PlaybackLog
            {
                FoodStallId = request.FoodStallId,
                LanguageCode = request.LanguageCode?.Trim() ?? "",
                TriggerType = string.IsNullOrWhiteSpace(request.TriggerType) ? "GPS" : request.TriggerType.Trim(),
                Status = "Started",
                StartedAt = DateTime.UtcNow,
                EndedAt = null,
                ActualDurationSeconds = 0
            };

            _context.PlaybackLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new StartPlaybackResponseDto
            {
                PlaybackLogId = log.Id,
                Message = "Playback started."
            });
        }

        [HttpPost("finish")]
        public async Task<IActionResult> FinishPlayback([FromBody] FinishPlaybackRequestDto request)
        {
            if (request == null || request.PlaybackLogId <= 0)
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ.");
            }

            var allowedStatuses = new[] { "Completed", "Stopped", "Interrupted" };
            if (!allowedStatuses.Contains(request.Status))
            {
                return BadRequest("Status không hợp lệ.");
            }

            var log = await _context.PlaybackLogs.FindAsync(request.PlaybackLogId);
            if (log == null)
            {
                return NotFound("Playback log not found.");
            }

            if (log.EndedAt != null && log.Status != "Started")
            {
                return Ok(new { message = "Playback log already finished." });
            }

            log.Status = request.Status;
            log.ActualDurationSeconds = Math.Max(0, request.ActualDurationSeconds);
            log.EndedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Playback finished." });
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<PlaybackLogAdminDto>>> GetPlaybackLogsAdmin()
        {
            var logs = await _context.PlaybackLogs
                .AsNoTracking()
                .Include(x => x.FoodStall)
                    .ThenInclude(fs => fs.Translations)
                        .ThenInclude(t => t.Language)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();

            var result = logs.Select(x =>
            {
                var viName = x.FoodStall?.Translations
                    .FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name ?? "";

                var enName = x.FoodStall?.Translations
                    .FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name ?? "";

                return new PlaybackLogAdminDto
                {
                    Id = x.Id,
                    FoodStallId = x.FoodStallId,
                    FoodStallAddress = x.FoodStall?.Address ?? "",
                    VietnameseName = viName,
                    EnglishName = enName,
                    LanguageCode = x.LanguageCode,
                    TriggerType = x.TriggerType,
                    Status = x.Status,
                    StartedAt = x.StartedAt,
                    EndedAt = x.EndedAt,
                    ActualDurationSeconds = x.ActualDurationSeconds
                };
            }).ToList();

            return Ok(result);
        }
    }
}