using AudioGuideAPI.Database;
using AudioGuideAPI.DTOs;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToursController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTours([FromQuery] string lang = "vi")
        {
            try
            {
                lang = string.IsNullOrWhiteSpace(lang) ? "vi" : lang.Trim().ToLower();

                var languages = await _context.Languages
                    .AsNoTracking()
                    .ToListAsync();

                var requestedLanguage = languages.FirstOrDefault(x => x.LanguageCode.ToLower() == lang);
                var defaultLanguage = languages.FirstOrDefault(x => x.IsDefault);

                var tours = await _context.Tours
                    .AsNoTracking()
                    .Include(t => t.Translations)
                        .ThenInclude(tr => tr.Language)
                    .Include(t => t.TourItems)
                        .ThenInclude(ti => ti.FoodStall)
                            .ThenInclude(fs => fs.Translations)
                                .ThenInclude(ft => ft.Language)
                    .Where(t => t.IsActive)
                    .ToListAsync();

                var result = tours
                    .Select(tour =>
                    {
                        var selectedTourTranslation = SelectTourTranslation(
                            tour.Translations,
                            requestedLanguage?.Id,
                            defaultLanguage?.Id);

                        var activeStops = tour.TourItems
                            .Where(ti => ti.FoodStall != null && ti.FoodStall.IsActive)
                            .OrderBy(ti => ti.OrderIndex)
                            .Select(ti =>
                            {
                                var selectedFoodStallTranslation = SelectFoodStallTranslation(
                                    ti.FoodStall.Translations,
                                    requestedLanguage?.Id,
                                    defaultLanguage?.Id);

                                return new TourStopDto
                                {
                                    FoodStallId = ti.FoodStallId,
                                    OrderIndex = ti.OrderIndex,
                                    Name = selectedFoodStallTranslation?.Name,
                                    Address = ti.FoodStall.Address,
                                    Latitude = ti.FoodStall.Latitude,
                                    Longitude = ti.FoodStall.Longitude,
                                    Radius = ti.FoodStall.Radius,
                                    MapLink = ti.FoodStall.MapLink
                                };
                            })
                            .ToList();

                        return new TourDto
                        {
                            Id = tour.Id,
                            Name = selectedTourTranslation?.Name,
                            Description = selectedTourTranslation?.Description,
                            LanguageCode = selectedTourTranslation?.Language?.LanguageCode
                                           ?? requestedLanguage?.LanguageCode
                                           ?? defaultLanguage?.LanguageCode
                                           ?? "vi",
                            StopCount = activeStops.Count,
                            Stops = activeStops
                        };
                    })
                    .Where(x => x.StopCount > 0)
                    .OrderBy(x => x.Id)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Lỗi máy chủ nội bộ: {ex.Message}"
                });
            }
        }

        private static TourTranslation? SelectTourTranslation(
            IEnumerable<TourTranslation> translations,
            int? requestedLanguageId,
            int? defaultLanguageId)
        {
            if (requestedLanguageId.HasValue)
            {
                var requested = translations.FirstOrDefault(x => x.LanguageId == requestedLanguageId.Value);
                if (requested != null)
                    return requested;
            }

            if (defaultLanguageId.HasValue)
            {
                var fallback = translations.FirstOrDefault(x => x.LanguageId == defaultLanguageId.Value);
                if (fallback != null)
                    return fallback;
            }

            return translations.FirstOrDefault();
        }

        private static FoodStallTranslation? SelectFoodStallTranslation(
            IEnumerable<FoodStallTranslation> translations,
            int? requestedLanguageId,
            int? defaultLanguageId)
        {
            if (requestedLanguageId.HasValue)
            {
                var requested = translations.FirstOrDefault(x => x.LanguageId == requestedLanguageId.Value);
                if (requested != null)
                    return requested;
            }

            if (defaultLanguageId.HasValue)
            {
                var fallback = translations.FirstOrDefault(x => x.LanguageId == defaultLanguageId.Value);
                if (fallback != null)
                    return fallback;
            }

            return translations.FirstOrDefault();
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<TourAdminDto>>> GetToursAdmin()
        {
            var tours = await _context.Tours
                .Include(t => t.Translations)
                    .ThenInclude(tr => tr.Language)
                .Include(t => t.TourItems)
                .ToListAsync();

            var result = tours.Select(t =>
            {
                var vi = t.Translations.FirstOrDefault(x => x.Language.LanguageCode == "vi");
                var en = t.Translations.FirstOrDefault(x => x.Language.LanguageCode == "en");

                return new TourAdminDto
                {
                    Id = t.Id,
                    IsActive = t.IsActive,

                    VietnameseName = vi?.Name ?? "",
                    VietnameseDescription = vi?.Description,

                    EnglishName = en?.Name ?? "",
                    EnglishDescription = en?.Description,

                    Items = t.TourItems
                        .OrderBy(x => x.OrderIndex)
                        .Select(x => new TourItemDto
                        {
                            FoodStallId = x.FoodStallId,
                            OrderIndex = x.OrderIndex
                        }).ToList()
                };
            });

            return Ok(result);
        }

        [HttpPost("admin")]
        public async Task<IActionResult> CreateTourAdmin([FromBody] TourAdminDto dto)
        {
            var tour = new Tour
            {
                IsActive = dto.IsActive
            };

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            await SaveTranslations(tour.Id, dto);
            await SaveItems(tour.Id, dto.Items);

            await _context.SaveChangesAsync();

            return Ok(tour.Id);
        }

        [HttpPut("admin/{id}")]
        public async Task<IActionResult> UpdateTourAdmin(int id, [FromBody] TourAdminDto dto)
        {
            var tour = await _context.Tours
                .Include(t => t.TourItems)
                .Include(t => t.Translations)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null)
                return NotFound();

            tour.IsActive = dto.IsActive;

            _context.TourItems.RemoveRange(tour.TourItems);

            await SaveTranslations(id, dto);
            await SaveItems(id, dto.Items);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("admin/{id:int}")]
        public async Task<IActionResult> DeleteTourAdmin(int id)
        {
            var tour = await _context.Tours.FindAsync(id);

            if (tour == null)
                return NotFound();

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task SaveTranslations(int tourId, TourAdminDto dto)
        {
            var viLang = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "vi");
            var enLang = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLang == null || enLang == null)
            {
                throw new Exception("Languages vi/en not found");
            }

            await UpsertTranslation(tourId, viLang.Id, dto.VietnameseName, dto.VietnameseDescription);
            await UpsertTranslation(tourId, enLang.Id, dto.EnglishName, dto.EnglishDescription);
        }

        private async Task UpsertTranslation(int tourId, int languageId, string name, string? desc)
        {
            var t = await _context.TourTranslations
                .FirstOrDefaultAsync(x => x.TourId == tourId && x.LanguageId == languageId);

            if (t == null)
            {
                t = new TourTranslation
                {
                    TourId = tourId,
                    LanguageId = languageId
                };

                _context.TourTranslations.Add(t);
            }

            t.Name = name;
            t.Description = desc;
        }

        private async Task SaveItems(int tourId, List<TourItemDto> items)
        {
            foreach (var item in items)
            {
                _context.TourItems.Add(new TourItem
                {
                    TourId = tourId,
                    FoodStallId = item.FoodStallId,
                    OrderIndex = item.OrderIndex
                });
            }
        }
    }
}