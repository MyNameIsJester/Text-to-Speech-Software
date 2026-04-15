using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AudioGuideAPI.Controllers
{
    public class LogRequestDto
    {
        public int FoodStallId { get; set; }
        public string LanguageCode { get; set; } = "";
        public string TriggerType { get; set; } = "";
        public int DurationSeconds { get; set; }
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

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LogRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ.");
            }

            try
            {
                var newLog = new PlaybackLog
                {
                    FoodStallId = request.FoodStallId,
                    LanguageCode = request.LanguageCode,
                    TriggerType = request.TriggerType,
                    DurationSeconds = request.DurationSeconds,
                    PlayedAt = DateTime.UtcNow
                };

                _context.PlaybackLogs.Add(newLog);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lưu lịch sử nghe audio thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }
    }
}