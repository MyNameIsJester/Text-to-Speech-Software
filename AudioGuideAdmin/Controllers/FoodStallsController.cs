using AudioGuideAdmin.ViewModels.FoodStalls;
using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class FoodStallsController : Controller
    {
        private readonly AppDbContext _context;

        public FoodStallsController(AppDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index(string? searchTerm, string statusFilter = "all")
        {
            var query = _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim().ToLower();

                query = query.Where(x =>
                    (x.Address != null && x.Address.ToLower().Contains(keyword)) ||
                    x.Translations.Any(t =>
                        (t.Name != null && t.Name.ToLower().Contains(keyword))
                    ));
            }

            if (statusFilter == "active")
            {
                query = query.Where(x => x.IsActive);
            }
            else if (statusFilter == "inactive")
            {
                query = query.Where(x => !x.IsActive);
            }

            var foodStalls = await query
                .OrderBy(x => x.Id)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.StatusFilter = statusFilter;

            return View(foodStalls);
        }

        // GET: /FoodStalls/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new FoodStall
            {
                Radius = 35,
                Priority = 0,
                IsActive = true
            };

            return View(model);
        }

        // POST: /FoodStalls/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodStall model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var foodStall = new FoodStall
            {
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Radius = model.Radius,
                Priority = model.Priority,
                ImageUrl = model.ImageUrl,
                Address = model.Address,
                PriceRange = model.PriceRange,
                MapLink = model.MapLink,
                IsActive = model.IsActive
            };

            _context.FoodStalls.Add(foodStall);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditTranslations), new { id = foodStall.Id });
        }

        // GET: /FoodStalls/Edit/1
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var foodStall = await _context.FoodStalls.FindAsync(id);

            if (foodStall == null)
                return NotFound();

            return View(foodStall);
        }

        // POST: /FoodStalls/Edit/1
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FoodStall model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var foodStall = await _context.FoodStalls.FindAsync(id);

            if (foodStall == null)
                return NotFound();

            foodStall.Latitude = model.Latitude;
            foodStall.Longitude = model.Longitude;
            foodStall.Radius = model.Radius;
            foodStall.Priority = model.Priority;
            foodStall.ImageUrl = model.ImageUrl;
            foodStall.Address = model.Address;
            foodStall.PriceRange = model.PriceRange;
            foodStall.MapLink = model.MapLink;
            foodStall.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /FoodStalls/Delete/1
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var foodStall = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (foodStall == null)
                return NotFound();

            return View(foodStall);
        }

        // POST: /FoodStalls/Delete/1
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodStall = await _context.FoodStalls.FindAsync(id);

            if (foodStall == null)
                return NotFound();

            _context.FoodStalls.Remove(foodStall);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /FoodStalls/EditTranslations/1
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTranslations(int id)
        {
            var foodStall = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (foodStall == null)
                return NotFound();

            var viLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "vi");
            var enLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLanguage == null || enLanguage == null)
                return BadRequest("Languages vi/en not found.");

            var viTranslation = foodStall.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi");
            var enTranslation = foodStall.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en");

            var vm = new FoodStallTranslationsEditViewModel
            {
                FoodStallId = foodStall.Id,
                StallAddress = foodStall.Address ?? "",

                Vietnamese = new TranslationInputViewModel
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

                English = new TranslationInputViewModel
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

            return View(vm);
        }

        // POST: /FoodStalls/EditTranslations/1
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTranslations(int id, FoodStallTranslationsEditViewModel model)
        {
            if (id != model.FoodStallId)
                return NotFound();

            var foodStall = await _context.FoodStalls
                .FirstOrDefaultAsync(x => x.Id == id);

            if (foodStall == null)
                return NotFound();

            await UpsertTranslation(foodStall.Id, model.Vietnamese);
            await UpsertTranslation(foodStall.Id, model.English);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task UpsertTranslation(int foodStallId, TranslationInputViewModel input)
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
    }
}