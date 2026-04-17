using AudioGuideAdmin.ViewModels.Dashboard;
using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalFoodStalls = await _context.FoodStalls.CountAsync();
            var activeFoodStalls = await _context.FoodStalls.CountAsync(x => x.IsActive);
            var totalPlaybackLogs = await _context.PlaybackLogs.CountAsync();
            var totalQrMappings = await _context.QrMappings.CountAsync();
            var totalTours = await _context.Tours.CountAsync();

            var gpsLogs = await _context.PlaybackLogs.CountAsync(x => x.TriggerType == "GPS");
            var qrLogs = await _context.PlaybackLogs.CountAsync(x => x.TriggerType == "QR");
            var viLogs = await _context.PlaybackLogs.CountAsync(x => x.LanguageCode == "vi");
            var enLogs = await _context.PlaybackLogs.CountAsync(x => x.LanguageCode == "en");

            var recentPlaybackLogs = await _context.PlaybackLogs
                .Include(x => x.FoodStall)
                    .ThenInclude(fs => fs.Translations)
                        .ThenInclude(t => t.Language)
                .OrderByDescending(x => x.PlayedAt)
                .Take(5)
                .Select(x => new RecentPlaybackLogViewModel
                {
                    Id = x.Id,
                    FoodStallId = x.FoodStallId,
                    FoodStallName = x.FoodStall.Translations
                        .Where(t => t.Language.LanguageCode == "vi")
                        .Select(t => t.Name)
                        .FirstOrDefault()
                        ?? x.FoodStall.Translations
                            .Where(t => t.Language.LanguageCode == "en")
                            .Select(t => t.Name)
                            .FirstOrDefault()
                        ?? $"Food Stall {x.FoodStallId}",
                    FoodStallAddress = x.FoodStall.Address ?? "-",
                    LanguageCode = x.LanguageCode,
                    TriggerType = x.TriggerType,
                    PlayedAt = x.PlayedAt,
                    DurationSeconds = x.DurationSeconds
                })
                .ToListAsync();

            var topFoodStallsRaw = await _context.PlaybackLogs
                .GroupBy(x => x.FoodStallId)
                .Select(g => new
                {
                    FoodStallId = g.Key,
                    PlaybackCount = g.Count()
                })
                .OrderByDescending(x => x.PlaybackCount)
                .Take(5)
                .ToListAsync();

            var topFoodStallIds = topFoodStallsRaw.Select(x => x.FoodStallId).ToList();

            var topFoodStallMap = await _context.FoodStalls
                .Where(x => topFoodStallIds.Contains(x.Id))
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .ToDictionaryAsync(
                    x => x.Id,
                    x =>
                    {
                        var viName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name;
                        var enName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name;

                        return new TopFoodStallViewModel
                        {
                            FoodStallId = x.Id,
                            FoodStallName = !string.IsNullOrWhiteSpace(viName)
                                ? viName
                                : !string.IsNullOrWhiteSpace(enName)
                                    ? enName
                                    : $"Food Stall {x.Id}",
                            FoodStallAddress = x.Address ?? "-",
                            PlaybackCount = 0
                        };
                    });

            var topFoodStalls = topFoodStallsRaw
                .Where(x => topFoodStallMap.ContainsKey(x.FoodStallId))
                .Select(x =>
                {
                    var item = topFoodStallMap[x.FoodStallId];
                    item.PlaybackCount = x.PlaybackCount;
                    return item;
                })
                .ToList();

            var vm = new DashboardViewModel
            {
                TotalFoodStalls = totalFoodStalls,
                ActiveFoodStalls = activeFoodStalls,
                TotalPlaybackLogs = totalPlaybackLogs,
                TotalQrMappings = totalQrMappings,
                TotalTours = totalTours,

                GpsLogs = gpsLogs,
                QrLogs = qrLogs,
                ViLogs = viLogs,
                EnLogs = enLogs,

                RecentPlaybackLogs = recentPlaybackLogs,
                TopFoodStalls = topFoodStalls
            };

            return View(vm);
        }
    }
}