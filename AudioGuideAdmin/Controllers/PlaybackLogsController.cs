using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class PlaybackLogsController : Controller
    {
        private readonly AppDbContext _context;

        public PlaybackLogsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? languageCode,
            string? triggerType,
            int? foodStallId)
        {
            var query = _context.PlaybackLogs
                .Include(x => x.FoodStall)
                    .ThenInclude(fs => fs.Translations)
                        .ThenInclude(t => t.Language)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                query = query.Where(x => x.LanguageCode == languageCode);
            }

            if (!string.IsNullOrWhiteSpace(triggerType))
            {
                query = query.Where(x => x.TriggerType == triggerType);
            }

            if (foodStallId.HasValue)
            {
                query = query.Where(x => x.FoodStallId == foodStallId.Value);
            }

            var logs = await query
                .OrderByDescending(x => x.PlayedAt)
                .ToListAsync();

            var totalLogs = logs.Count;
            var gpsLogs = logs.Count(x => x.TriggerType == "GPS");
            var qrLogs = logs.Count(x => x.TriggerType == "QR");
            var viLogs = logs.Count(x => x.LanguageCode == "vi");
            var enLogs = logs.Count(x => x.LanguageCode == "en");

            var foodStalls = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .OrderBy(x => x.Id)
                .ToListAsync();

            var foodStallOptions = foodStalls.Select(x =>
            {
                var viName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name;
                var enName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name;

                var displayName = !string.IsNullOrWhiteSpace(viName)
                    ? viName
                    : !string.IsNullOrWhiteSpace(enName)
                        ? enName
                        : (x.Address ?? $"Food Stall {x.Id}");

                return new PlaybackFoodStallOptionViewModel
                {
                    Id = x.Id,
                    Label = $"{x.Id} - {displayName}",
                    SearchText = $"{displayName} {x.Address}".Trim()
                };
            }).ToList();

            string selectedFoodStallLabel = "";
            if (foodStallId.HasValue)
            {
                selectedFoodStallLabel = foodStallOptions
                    .FirstOrDefault(x => x.Id == foodStallId.Value)?.Label ?? "";
            }

            ViewBag.LanguageCode = languageCode;
            ViewBag.TriggerType = triggerType;
            ViewBag.FoodStallId = foodStallId;
            ViewBag.SelectedFoodStallLabel = selectedFoodStallLabel;

            ViewBag.TotalLogs = totalLogs;
            ViewBag.GpsLogs = gpsLogs;
            ViewBag.QrLogs = qrLogs;
            ViewBag.ViLogs = viLogs;
            ViewBag.EnLogs = enLogs;

            ViewBag.FoodStallOptions = foodStallOptions;

            return View(logs);
        }

        public class PlaybackFoodStallOptionViewModel
        {
            public int Id { get; set; }
            public string Label { get; set; } = "";
            public string SearchText { get; set; } = "";
        }
    }
}