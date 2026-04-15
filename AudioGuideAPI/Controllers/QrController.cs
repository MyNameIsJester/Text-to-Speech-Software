using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QrController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resolve")]
        public async Task<IActionResult> Resolve([FromQuery] string code, [FromQuery] string lang = "vi")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { message = "Code is required." });
            }

            var qrMapping = await _context.QrMappings
                .Include(x => x.FoodStall)
                    .ThenInclude(fs => fs.Translations)
                        .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.CodeValue == code && x.IsActive);

            if (qrMapping == null)
            {
                return NotFound(new { message = "QR code not found or inactive." });
            }

            var foodStall = qrMapping.FoodStall;

            var translation = foodStall.Translations
                .FirstOrDefault(t => t.Language.LanguageCode == lang);

            if (translation == null)
            {
                translation = foodStall.Translations
                    .FirstOrDefault(t => t.Language.LanguageCode == "vi");
            }

            if (translation == null)
            {
                return NotFound(new { message = "No translation found for this food stall." });
            }

            return Ok(new
            {
                foodStallId = foodStall.Id,
                qrCode = qrMapping.CodeValue,
                language = translation.Language.LanguageCode,
                name = translation.Name,
                description = translation.Description,
                ttsScript = translation.Description,
                specialty = translation.Specialty,
                audioUrl = translation.AudioUrl,
                latitude = foodStall.Latitude,
                longitude = foodStall.Longitude,
                radius = foodStall.Radius,
                priority = foodStall.Priority,
                imageUrl = foodStall.ImageUrl,
                address = foodStall.Address,
                priceRange = foodStall.PriceRange,
                mapLink = foodStall.MapLink,
                isActive = foodStall.IsActive,
                triggerType = "QR"
            });
        }
    }
}