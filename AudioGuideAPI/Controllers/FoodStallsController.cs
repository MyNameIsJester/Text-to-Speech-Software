using AudioGuideAPI.Database;
using AudioGuideAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodStallsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FoodStallsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodStallMobileDto>>> GetFoodStalls([FromQuery] string? lang = "vi")
        {
            var defaultLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsDefault);

            if (defaultLanguage == null)
            {
                return BadRequest("No default language configured.");
            }

            var selectedLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LanguageCode == lang);

            var selectedLanguageId = selectedLanguage?.Id ?? defaultLanguage.Id;

            var foodStalls = await _context.FoodStalls
                .AsNoTracking()
                .Where(fs => fs.IsActive)
                .Select(fs => new
                {
                    FoodStall = fs,
                    RequestedTranslation = fs.Translations
                        .Where(t => t.LanguageId == selectedLanguageId)
                        .Select(t => new
                        {
                            t.Name,
                            t.Description,
                            t.Specialty,
                            t.AudioUrl,
                            LanguageCode = t.Language.LanguageCode
                        })
                        .FirstOrDefault(),

                    DefaultTranslation = fs.Translations
                        .Where(t => t.LanguageId == defaultLanguage.Id)
                        .Select(t => new
                        {
                            t.Name,
                            t.Description,
                            t.Specialty,
                            t.AudioUrl,
                            LanguageCode = t.Language.LanguageCode
                        })
                        .FirstOrDefault()
                })
                .OrderBy(x => x.FoodStall.Priority)
                .ToListAsync();

            var result = foodStalls.Select(x =>
            {
                var tr = x.RequestedTranslation ?? x.DefaultTranslation;

                return new FoodStallMobileDto
                {
                    Id = x.FoodStall.Id,
                    Name = tr?.Name,
                    Address = x.FoodStall.Address,
                    Specialty = tr?.Specialty,
                    PriceRange = x.FoodStall.PriceRange,
                    ImageUrl = ToAbsoluteUrl(x.FoodStall.ImageUrl),
                    Latitude = x.FoodStall.Latitude,
                    Longitude = x.FoodStall.Longitude,
                    Radius = x.FoodStall.Radius,
                    Description = tr?.Description,
                    AudioUrl = ToAbsoluteUrl(tr?.AudioUrl),
                    Priority = x.FoodStall.Priority,
                    MapLink = x.FoodStall.MapLink,
                    LanguageCode = tr?.LanguageCode
                };
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodStallMobileDto>> GetFoodStallById(int id, [FromQuery] string? lang = "vi")
        {
            var defaultLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsDefault);

            if (defaultLanguage == null)
            {
                return BadRequest("No default language configured.");
            }

            var selectedLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LanguageCode == lang);

            var selectedLanguageId = selectedLanguage?.Id ?? defaultLanguage.Id;

            var item = await _context.FoodStalls
                .AsNoTracking()
                .Where(fs => fs.Id == id && fs.IsActive)
                .Select(fs => new
                {
                    FoodStall = fs,
                    RequestedTranslation = fs.Translations
                        .Where(t => t.LanguageId == selectedLanguageId)
                        .Select(t => new
                        {
                            t.Name,
                            t.Description,
                            t.Specialty,
                            t.AudioUrl,
                            LanguageCode = t.Language.LanguageCode
                        })
                        .FirstOrDefault(),

                    DefaultTranslation = fs.Translations
                        .Where(t => t.LanguageId == defaultLanguage.Id)
                        .Select(t => new
                        {
                            t.Name,
                            t.Description,
                            t.Specialty,
                            t.AudioUrl,
                            LanguageCode = t.Language.LanguageCode
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            var tr = item.RequestedTranslation ?? item.DefaultTranslation;

            var dto = new FoodStallMobileDto
            {
                Id = item.FoodStall.Id,
                Name = tr?.Name,
                Address = item.FoodStall.Address,
                Specialty = tr?.Specialty,
                PriceRange = item.FoodStall.PriceRange,
                ImageUrl = item.FoodStall.ImageUrl,
                Latitude = item.FoodStall.Latitude,
                Longitude = item.FoodStall.Longitude,
                Radius = item.FoodStall.Radius,
                Description = tr?.Description,
                AudioUrl = tr?.AudioUrl,
                Priority = item.FoodStall.Priority,
                MapLink = item.FoodStall.MapLink,
                LanguageCode = tr?.LanguageCode
            };

            return Ok(dto);
        }

        private string? ToAbsoluteUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{path}";
        }
    }
}