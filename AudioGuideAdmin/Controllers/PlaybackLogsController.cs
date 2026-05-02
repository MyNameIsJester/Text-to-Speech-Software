using AudioGuideAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class PlaybackLogsController : Controller
    {
        private readonly AdminPlaybackLogApiService _playbackLogApiService;

        public PlaybackLogsController(AdminPlaybackLogApiService playbackLogApiService)
        {
            _playbackLogApiService = playbackLogApiService;
        }

        public async Task<IActionResult> Index(
            string? languageCode,
            string? triggerType,
            string? status,
            string? foodStallKeyword)
        {
            var logs = await _playbackLogApiService.GetPlaybackLogsAsync();
            var query = logs.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                query = query.Where(x => x.LanguageCode == languageCode);
            }

            if (!string.IsNullOrWhiteSpace(triggerType))
            {
                query = query.Where(x => x.TriggerType == triggerType);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(foodStallKeyword))
            {
                var keyword = foodStallKeyword.Trim().ToLowerInvariant();

                query = query.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.FoodStall?.Address) &&
                     x.FoodStall.Address.ToLowerInvariant().Contains(keyword)) ||
                    (x.FoodStall?.Translations?.Any(t =>
                        !string.IsNullOrWhiteSpace(t.Name) &&
                        t.Name.ToLowerInvariant().Contains(keyword)) ?? false)
                );
            }

            var filteredLogs = query
                .OrderByDescending(x => x.StartedAt)
                .ToList();

            ViewBag.LanguageCode = languageCode ?? "";
            ViewBag.TriggerType = triggerType ?? "";
            ViewBag.Status = status ?? "";
            ViewBag.FoodStallKeyword = foodStallKeyword ?? "";

            ViewBag.TotalLogs = filteredLogs.Count;
            ViewBag.ViLogs = filteredLogs.Count(x => x.LanguageCode == "vi");
            ViewBag.EnLogs = filteredLogs.Count(x => x.LanguageCode == "en");
            ViewBag.CompletedLogs = filteredLogs.Count(x => x.Status == "Completed");
            ViewBag.StoppedLogs = filteredLogs.Count(x => x.Status == "Stopped");
            ViewBag.InterruptedLogs = filteredLogs.Count(x => x.Status == "Interrupted");

            return View(filteredLogs);
        }
    }
}