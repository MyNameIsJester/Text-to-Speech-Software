using AudioGuideAdmin.Services;
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
        private readonly WebVisitApiService _webVisitApiService;

        public DashboardController(AppDbContext context, WebVisitApiService webVisitApiService)
        {
            _context = context;
            _webVisitApiService = webVisitApiService;
        }

        public async Task<IActionResult> Index()
        {
            var totalFoodStalls = await _context.FoodStalls.CountAsync();
            var activeFoodStalls = await _context.FoodStalls.CountAsync(x => x.IsActive);
            var totalPlaybackLogs = await _context.PlaybackLogs.CountAsync();
            var totalQrMappings = await _context.QrMappings.CountAsync();
            var totalTours = await _context.Tours.CountAsync();

            var gpsLogs = await _context.PlaybackLogs.CountAsync(x => x.TriggerType == "GPS");
            var demoLogs = await _context.PlaybackLogs.CountAsync(x => x.TriggerType == "DEMO");
            var viLogs = await _context.PlaybackLogs.CountAsync(x => x.LanguageCode == "vi");
            var enLogs = await _context.PlaybackLogs.CountAsync(x => x.LanguageCode == "en");

            var completedLogs = await _context.PlaybackLogs.CountAsync(x => x.Status == "Completed");
            var stoppedLogs = await _context.PlaybackLogs.CountAsync(x => x.Status == "Stopped");
            var interruptedLogs = await _context.PlaybackLogs.CountAsync(x => x.Status == "Interrupted");

            var totalActualListeningSeconds = await _context.PlaybackLogs
                .SumAsync(x => (int?)x.ActualDurationSeconds) ?? 0;

            var recentPlaybackLogs = await _context.PlaybackLogs
                .Include(x => x.FoodStall)
                    .ThenInclude(fs => fs.Translations)
                        .ThenInclude(t => t.Language)
                .OrderByDescending(x => x.StartedAt)
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
                    Status = x.Status,
                    StartedAt = x.StartedAt,
                    EndedAt = x.EndedAt,
                    ActualDurationSeconds = x.ActualDurationSeconds
                })
                .ToListAsync();

            var topFoodStallsRaw = await _context.PlaybackLogs
                .Where(x => x.Status == "Completed")
                .GroupBy(x => x.FoodStallId)
                .Select(g => new
                {
                    FoodStallId = g.Key,
                    PlaybackCount = g.Count(),
                    TotalActualDurationSeconds = g.Sum(x => x.ActualDurationSeconds)
                })
                .OrderByDescending(x => x.PlaybackCount)
                .ThenByDescending(x => x.TotalActualDurationSeconds)
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
                            PlaybackCount = 0,
                            TotalActualDurationSeconds = 0
                        };
                    });

            var topFoodStalls = topFoodStallsRaw
                .Where(x => topFoodStallMap.ContainsKey(x.FoodStallId))
                .Select(x =>
                {
                    var item = topFoodStallMap[x.FoodStallId];
                    item.PlaybackCount = x.PlaybackCount;
                    item.TotalActualDurationSeconds = x.TotalActualDurationSeconds;
                    return item;
                })
                .ToList();

            WebVisitStatsViewModel webVisitStats;
            try
            {
                webVisitStats = await _webVisitApiService.GetStatsAsync();
            }
            catch
            {
                webVisitStats = new WebVisitStatsViewModel();
            }

            var vm = new DashboardViewModel
            {
                TotalFoodStalls = totalFoodStalls,
                ActiveFoodStalls = activeFoodStalls,
                TotalPlaybackLogs = totalPlaybackLogs,
                TotalQrMappings = totalQrMappings,
                TotalTours = totalTours,

                TotalWebVisits = webVisitStats.TotalVisits,
                ActiveWebUsers = webVisitStats.ActiveUsers,

                GpsLogs = gpsLogs,
                DemoLogs = demoLogs,
                ViLogs = viLogs,
                EnLogs = enLogs,

                CompletedLogs = completedLogs,
                StoppedLogs = stoppedLogs,
                InterruptedLogs = interruptedLogs,
                TotalActualListeningSeconds = totalActualListeningSeconds,

                RecentPlaybackLogs = recentPlaybackLogs,
                TopFoodStalls = topFoodStalls
            };

            return View(vm);
        }
    }
}