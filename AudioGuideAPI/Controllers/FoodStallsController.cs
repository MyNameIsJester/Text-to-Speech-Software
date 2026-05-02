using AudioGuideAPI.Database;
using AudioGuideAPI.DTOs;
using AudioGuideAPI.Models;
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

        [HttpGet("{id:int}")]
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

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<FoodStallAdminDto>>> GetFoodStallsForAdmin()
        {
            var items = await _context.FoodStalls
                .AsNoTracking()
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapAdminDto));
        }

        [HttpGet("admin/{id:int}")]
        public async Task<ActionResult<FoodStallAdminDto>> GetFoodStallForAdminById(int id)
        {
            var item = await _context.FoodStalls
                .AsNoTracking()
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(MapAdminDto(item));
        }

        [HttpPost("admin")]
        public async Task<ActionResult<FoodStallAdminDto>> CreateFoodStallForAdmin([FromBody] FoodStallAdminUpsertDto dto)
        {
            var entity = new FoodStall
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Radius = dto.Radius,
                Priority = dto.Priority,
                ImageUrl = dto.ImageUrl,
                Address = dto.Address,
                PriceRange = dto.PriceRange,
                MapLink = dto.MapLink,
                IsActive = dto.IsActive,
                OwnerUserId = string.IsNullOrWhiteSpace(dto.OwnerUserId) ? null : dto.OwnerUserId
            };

            _context.FoodStalls.Add(entity);
            await _context.SaveChangesAsync();

            entity = await _context.FoodStalls
                .AsNoTracking()
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetFoodStallForAdminById), new { id = entity.Id }, MapAdminDto(entity));
        }

        [HttpPut("admin/{id:int}")]
        public async Task<IActionResult> UpdateFoodStallForAdmin(int id, [FromBody] FoodStallAdminUpsertDto dto)
        {
            var entity = await _context.FoodStalls.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.Radius = dto.Radius;
            entity.Priority = dto.Priority;
            entity.ImageUrl = dto.ImageUrl;
            entity.Address = dto.Address;
            entity.PriceRange = dto.PriceRange;
            entity.MapLink = dto.MapLink;
            entity.IsActive = dto.IsActive;
            entity.OwnerUserId = string.IsNullOrWhiteSpace(dto.OwnerUserId) ? null : dto.OwnerUserId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("admin/{id:int}")]
        public async Task<IActionResult> DeleteFoodStallForAdmin(int id)
        {
            var entity = await _context.FoodStalls.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.FoodStalls.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("admin/{id:int}/translations-editor")]
        public async Task<ActionResult<FoodStallTranslationsEditorDto>> GetTranslationsEditor(int id)
        {
            var foodStall = await _context.FoodStalls
                .AsNoTracking()
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (foodStall == null)
            {
                return NotFound();
            }

            var viLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LanguageCode == "vi");

            var enLanguage = await _context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLanguage == null || enLanguage == null)
            {
                return BadRequest("Languages vi/en not found.");
            }

            var viTranslation = foodStall.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi");
            var enTranslation = foodStall.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en");

            var dto = new FoodStallTranslationsEditorDto
            {
                FoodStallId = foodStall.Id,
                StallAddress = foodStall.Address ?? "",
                Vietnamese = new FoodStallTranslationInputDto
                {
                    TranslationId = viTranslation?.Id,
                    LanguageId = viLanguage.Id,
                    LanguageCode = viLanguage.LanguageCode,
                    DisplayName = viLanguage.DisplayName,
                    Name = viTranslation?.Name ?? "",
                    Description = viTranslation?.Description,
                    Specialty = viTranslation?.Specialty,
                    AudioUrl = viTranslation?.AudioUrl
                },
                English = new FoodStallTranslationInputDto
                {
                    TranslationId = enTranslation?.Id,
                    LanguageId = enLanguage.Id,
                    LanguageCode = enLanguage.LanguageCode,
                    DisplayName = enLanguage.DisplayName,
                    Name = enTranslation?.Name ?? "",
                    Description = enTranslation?.Description,
                    Specialty = enTranslation?.Specialty,
                    AudioUrl = enTranslation?.AudioUrl
                }
            };

            return Ok(dto);
        }

        [HttpPut("admin/{id:int}/translations")]
        public async Task<IActionResult> SaveTranslations(int id, [FromBody] FoodStallTranslationsEditorDto dto)
        {
            if (id != dto.FoodStallId)
            {
                return BadRequest("Food stall id mismatch.");
            }

            var foodStall = await _context.FoodStalls.FirstOrDefaultAsync(x => x.Id == id);

            if (foodStall == null)
            {
                return NotFound();
            }

            await UpsertTranslation(id, dto.Vietnamese);
            await UpsertTranslation(id, dto.English);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task UpsertTranslation(int foodStallId, FoodStallTranslationInputDto input)
        {
            var translation = await _context.FoodStallTranslations
                .FirstOrDefaultAsync(x => x.FoodStallId == foodStallId && x.LanguageId == input.LanguageId);

            if (translation == null)
            {
                translation = new FoodStallTranslation
                {
                    FoodStallId = foodStallId,
                    LanguageId = input.LanguageId
                };

                _context.FoodStallTranslations.Add(translation);
            }

            translation.Name = input.Name?.Trim() ?? "";
            translation.Description = input.Description?.Trim();
            translation.Specialty = input.Specialty?.Trim();
            translation.AudioUrl = string.IsNullOrWhiteSpace(input.AudioUrl)
                ? null
                : input.AudioUrl.Trim();
        }

        private FoodStallAdminDto MapAdminDto(FoodStall item)
        {
            return new FoodStallAdminDto
            {
                Id = item.Id,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Radius = item.Radius,
                Priority = item.Priority,
                ImageUrl = item.ImageUrl,
                Address = item.Address,
                PriceRange = item.PriceRange,
                MapLink = item.MapLink,
                IsActive = item.IsActive,
                OwnerUserId = item.OwnerUserId,
                Translations = item.Translations
                    .OrderBy(t => t.LanguageId)
                    .Select(t => new FoodStallAdminTranslationDto
                    {
                        TranslationId = t.Id,
                        LanguageId = t.LanguageId,
                        LanguageCode = t.Language.LanguageCode,
                        DisplayName = t.Language.DisplayName,
                        Name = t.Name,
                        Description = t.Description,
                        Specialty = t.Specialty,
                        AudioUrl = t.AudioUrl
                    })
                    .ToList()
            };
        }

        private string? ToAbsoluteUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (Uri.TryCreate(path, UriKind.Absolute, out _))
                return path;

            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{path}";
        }
    }
}