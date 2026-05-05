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
        private readonly AdminPlaybackLogApiService _playbackLogApiService;

        public DashboardController(
            AppDbContext context,
            WebVisitApiService webVisitApiService,
            AdminPlaybackLogApiService playbackLogApiService)
        {
            _context = context;
            _webVisitApiService = webVisitApiService;
            _playbackLogApiService = playbackLogApiService;
        }

        public async Task<IActionResult> Index()
        {
            var totalFoodStalls = await _context.FoodStalls.CountAsync();
            var activeFoodStalls = await _context.FoodStalls.CountAsync(x => x.IsActive);
            var totalQrMappings = await _context.QrMappings.CountAsync();
            var totalTours = await _context.Tours.CountAsync();

            var playbackLogs = await _playbackLogApiService.GetPlaybackLogsAsync();

            var totalPlaybackLogs = playbackLogs.Count;

            var demoLogs = playbackLogs.Count(x => x.TriggerType == "DEMO");
            var tourLogs = playbackLogs.Count(x => x.TriggerType == "TOUR");

            var viLogs = playbackLogs.Count(x => x.LanguageCode == "vi");
            var enLogs = playbackLogs.Count(x => x.LanguageCode == "en");

            var completedLogs = playbackLogs.Count(x => x.Status == "Completed");
            var stoppedLogs = playbackLogs.Count(x => x.Status == "Stopped");
            var interruptedLogs = playbackLogs.Count(x => x.Status == "Interrupted");

            var totalActualListeningSeconds = playbackLogs.Sum(x => x.ActualDurationSeconds);

            var averageListeningSeconds = completedLogs > 0
                ? (await _context.PlaybackLogs
                    .Where(x => x.Status == "Completed")
                    .AverageAsync(x => (double?)x.ActualDurationSeconds) ?? 0)
                : 0;

            var completionRatePercent = totalPlaybackLogs > 0
                ? Math.Round((double)completedLogs / totalPlaybackLogs * 100, 1)
                : 0;

            var recentPlaybackLogs = playbackLogs
                .OrderByDescending(x => x.StartedAt)
                .Take(5)
                .Select(x => new RecentPlaybackLogViewModel
                {
                    Id = x.Id,
                    FoodStallId = x.FoodStallId,
                    FoodStallName = x.FoodStall?.Translations
                        .FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name
                        ?? x.FoodStall?.Translations
                            .FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name
                        ?? $"Food Stall {x.FoodStallId}",
                    FoodStallAddress = x.FoodStall?.Address ?? "-",
                    LanguageCode = x.LanguageCode,
                    TriggerType = x.TriggerType,
                    Status = x.Status,
                    StartedAt = x.StartedAt,
                    EndedAt = x.EndedAt,
                    ActualDurationSeconds = x.ActualDurationSeconds
                })
                .ToList();

            var topFoodStalls = playbackLogs
                .Where(x => x.Status == "Completed")
                .GroupBy(x => x.FoodStallId)
                .Select(g =>
                {
                    var first = g.First();

                    var name = first.FoodStall?.Translations
                        .FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name
                        ?? first.FoodStall?.Translations
                            .FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name
                        ?? $"Food Stall {g.Key}";

                    return new TopFoodStallViewModel
                    {
                        FoodStallId = g.Key,
                        FoodStallName = name,
                        FoodStallAddress = first.FoodStall?.Address ?? "-",
                        PlaybackCount = g.Count(),
                        TotalActualDurationSeconds = g.Sum(x => x.ActualDurationSeconds)
                    };
                })
                .OrderByDescending(x => x.PlaybackCount)
                .ThenByDescending(x => x.TotalActualDurationSeconds)
                .Take(5)
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

                DemoLogs = demoLogs,
                TourLogs = tourLogs,
                ViLogs = viLogs,
                EnLogs = enLogs,

                CompletedLogs = completedLogs,
                StoppedLogs = stoppedLogs,
                InterruptedLogs = interruptedLogs,
                TotalActualListeningSeconds = totalActualListeningSeconds,
                AverageListeningSeconds = (int)Math.Round(averageListeningSeconds),
                CompletionRatePercent = completionRatePercent,

                RecentPlaybackLogs = recentPlaybackLogs,
                TopFoodStalls = topFoodStalls
            };

            return View(vm);
        }
    }
}