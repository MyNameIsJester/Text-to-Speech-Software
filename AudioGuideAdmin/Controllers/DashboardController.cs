using AudioGuideAdmin.ViewModels.Dashboard;
using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
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
                .OrderByDescending(x => x.PlayedAt)
                .Take(5)
                .Select(x => new RecentPlaybackLogViewModel
                {
                    Id = x.Id,
                    FoodStallId = x.FoodStallId,
                    FoodStallAddress = x.FoodStall.Address ?? "-",
                    LanguageCode = x.LanguageCode,
                    TriggerType = x.TriggerType,
                    PlayedAt = x.PlayedAt,
                    DurationSeconds = x.DurationSeconds
                })
                .ToListAsync();

            var topFoodStalls = await _context.PlaybackLogs
                .GroupBy(x => x.FoodStallId)
                .Select(g => new
                {
                    FoodStallId = g.Key,
                    PlaybackCount = g.Count()
                })
                .OrderByDescending(x => x.PlaybackCount)
                .Take(5)
                .Join(
                    _context.FoodStalls,
                    log => log.FoodStallId,
                    stall => stall.Id,
                    (log, stall) => new TopFoodStallViewModel
                    {
                        FoodStallId = stall.Id,
                        FoodStallAddress = stall.Address ?? "-",
                        PlaybackCount = log.PlaybackCount
                    })
                .ToListAsync();

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