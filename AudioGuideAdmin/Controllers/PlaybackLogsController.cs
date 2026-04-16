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
            string? foodStallKeyword)
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

            if (!string.IsNullOrWhiteSpace(foodStallKeyword))
            {
                var keyword = foodStallKeyword.Trim().ToLower();

                query = query.Where(x =>
                    x.FoodStall != null &&
                    (
                        (x.FoodStall.Address != null && x.FoodStall.Address.ToLower().Contains(keyword)) ||
                        x.FoodStall.Translations.Any(t =>
                            t.Name != null && t.Name.ToLower().Contains(keyword))
                    ));
            }

            var logs = await query
                .OrderByDescending(x => x.PlayedAt)
                .ToListAsync();

            var totalLogs = logs.Count;
            var viLogs = logs.Count(x => x.LanguageCode == "vi");
            var enLogs = logs.Count(x => x.LanguageCode == "en");

            ViewBag.LanguageCode = languageCode;
            ViewBag.FoodStallKeyword = foodStallKeyword;

            ViewBag.TotalLogs = totalLogs;
            ViewBag.ViLogs = viLogs;
            ViewBag.EnLogs = enLogs;

            return View(logs);
        }
    }
}