using AudioGuideAdmin.ViewModels.Tours;
using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class ToursController : Controller
    {
        private readonly AppDbContext _context;

        public ToursController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Tours
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .Include(x => x.TourItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                query = query.Where(x =>
                    x.Translations.Any(t =>
                        (t.Name != null && t.Name.ToLower().Contains(keyword)) ||
                        (t.Description != null && t.Description.ToLower().Contains(keyword))
                    ));
            }

            var tours = await query
                .OrderBy(x => x.Id)
                .ToListAsync();

            ViewBag.Search = search ?? "";

            return View(tours);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _context.Tours
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .Include(x => x.TourItems)
                    .ThenInclude(ti => ti.FoodStall)
                        .ThenInclude(fs => fs.Translations)
                            .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadFoodStallOptions();

            var viLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "vi");
            var enLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLanguage == null || enLanguage == null)
            {
                return BadRequest("Languages vi/en not found.");
            }

            var vm = new TourEditViewModel
            {
                IsActive = true,
                Vietnamese = new TourTranslationInputViewModel
                {
                    LanguageId = viLanguage.Id,
                    LanguageCode = viLanguage.LanguageCode,
                    DisplayName = viLanguage.DisplayName
                },
                English = new TourTranslationInputViewModel
                {
                    LanguageId = enLanguage.Id,
                    LanguageCode = enLanguage.LanguageCode,
                    DisplayName = enLanguage.DisplayName
                },
                Items = new List<TourItemInputViewModel>
                {
                    new TourItemInputViewModel { OrderIndex = 1 },
                    new TourItemInputViewModel { OrderIndex = 2 }
                }
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TourEditViewModel model)
        {
            await PopulateLanguageMetadata(model);

            if (!ModelState.IsValid)
            {
                await LoadFoodStallOptions();
                EnsureAtLeastTwoItems(model);
                return View(model);
            }

            var validItems = model.Items
                .Where(x => x.FoodStallId > 0)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            if (!validItems.Any())
            {
                ModelState.AddModelError("", "A tour must have at least one stop.");
                await LoadFoodStallOptions();
                EnsureAtLeastTwoItems(model);
                return View(model);
            }

            var tour = new Tour
            {
                IsActive = model.IsActive
            };

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            await UpsertTourTranslations(tour.Id, model);

            foreach (var item in validItems)
            {
                _context.TourItems.Add(new TourItem
                {
                    TourId = tour.Id,
                    FoodStallId = item.FoodStallId,
                    OrderIndex = item.OrderIndex
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _context.Tours
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .Include(x => x.TourItems)
                    .ThenInclude(ti => ti.FoodStall)
                        .ThenInclude(fs => fs.Translations)
                            .ThenInclude(t => t.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            await LoadFoodStallOptions();

            var viLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "vi");
            var enLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLanguage == null || enLanguage == null)
            {
                return BadRequest("Languages vi/en not found.");
            }

            var viTranslation = tour.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi");
            var enTranslation = tour.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en");

            var vm = new TourEditViewModel
            {
                Id = tour.Id,
                IsActive = tour.IsActive,

                Vietnamese = new TourTranslationInputViewModel
                {
                    TourTranslationId = viTranslation?.Id,
                    LanguageId = viLanguage.Id,
                    LanguageCode = viLanguage.LanguageCode,
                    DisplayName = viLanguage.DisplayName,
                    Name = viTranslation?.Name ?? "",
                    Description = viTranslation?.Description
                },

                English = new TourTranslationInputViewModel
                {
                    TourTranslationId = enTranslation?.Id,
                    LanguageId = enLanguage.Id,
                    LanguageCode = enLanguage.LanguageCode,
                    DisplayName = enLanguage.DisplayName,
                    Name = enTranslation?.Name ?? "",
                    Description = enTranslation?.Description
                },

                Items = tour.TourItems
                    .OrderBy(x => x.OrderIndex)
                    .Select(x =>
                    {
                        var viName = x.FoodStall.Translations
                            .FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name;
                        var enName = x.FoodStall.Translations
                            .FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name;

                        var label = !string.IsNullOrWhiteSpace(viName)
                            ? $"{x.FoodStallId} - {viName}"
                            : !string.IsNullOrWhiteSpace(enName)
                                ? $"{x.FoodStallId} - {enName}"
                                : $"{x.FoodStallId} - {x.FoodStall.Address}";

                        return new TourItemInputViewModel
                        {
                            TourItemId = x.Id,
                            FoodStallId = x.FoodStallId,
                            FoodStallLabel = label,
                            OrderIndex = x.OrderIndex
                        };
                    })
                    .ToList()
            };

            EnsureAtLeastTwoItems(vm);
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TourEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            await PopulateLanguageMetadata(model);

            if (!ModelState.IsValid)
            {
                await LoadFoodStallOptions();
                EnsureAtLeastTwoItems(model);
                return View(model);
            }

            var validItems = model.Items
                .Where(x => x.FoodStallId > 0)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            if (!validItems.Any())
            {
                ModelState.AddModelError("", "A tour must have at least one stop.");
                await LoadFoodStallOptions();
                EnsureAtLeastTwoItems(model);
                return View(model);
            }

            var tour = await _context.Tours
                .Include(x => x.TourItems)
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            tour.IsActive = model.IsActive;

            await UpsertTourTranslations(tour.Id, model);

            _context.TourItems.RemoveRange(tour.TourItems);

            foreach (var item in validItems)
            {
                _context.TourItems.Add(new TourItem
                {
                    TourId = tour.Id,
                    FoodStallId = item.FoodStallId,
                    OrderIndex = item.OrderIndex
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _context.Tours
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .Include(x => x.TourItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFoodStallOptions()
        {
            var foodStalls = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .OrderBy(x => x.Id)
                .ToListAsync();

            var options = foodStalls.Select(x =>
            {
                var viName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name;
                var enName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name;

                var displayName = !string.IsNullOrWhiteSpace(viName)
                    ? viName
                    : !string.IsNullOrWhiteSpace(enName)
                        ? enName
                        : (x.Address ?? $"Food Stall {x.Id}");

                return new FoodStallOptionViewModel
                {
                    Id = x.Id,
                    Label = $"{x.Id} - {displayName}",
                    SearchText = $"{displayName} {x.Address}".Trim(),
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                };
            }).ToList();

            ViewBag.FoodStallOptions = options;
        }

        private async Task PopulateLanguageMetadata(TourEditViewModel model)
        {
            var viLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "vi");
            var enLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.LanguageCode == "en");

            if (viLanguage != null)
            {
                model.Vietnamese.LanguageId = viLanguage.Id;
                model.Vietnamese.LanguageCode = viLanguage.LanguageCode;
                model.Vietnamese.DisplayName = viLanguage.DisplayName;
            }

            if (enLanguage != null)
            {
                model.English.LanguageId = enLanguage.Id;
                model.English.LanguageCode = enLanguage.LanguageCode;
                model.English.DisplayName = enLanguage.DisplayName;
            }
        }

        private async Task UpsertTourTranslations(int tourId, TourEditViewModel model)
        {
            await UpsertTranslation(tourId, model.Vietnamese);
            await UpsertTranslation(tourId, model.English);
        }

        private async Task UpsertTranslation(int tourId, TourTranslationInputViewModel input)
        {
            var translation = await _context.TourTranslations
                .FirstOrDefaultAsync(x => x.TourId == tourId && x.LanguageId == input.LanguageId);

            if (translation == null)
            {
                translation = new TourTranslation
                {
                    TourId = tourId,
                    LanguageId = input.LanguageId
                };

                _context.TourTranslations.Add(translation);
            }

            translation.Name = input.Name?.Trim() ?? "";
            translation.Description = input.Description?.Trim();
        }

        private static void EnsureAtLeastTwoItems(TourEditViewModel model)
        {
            model.Items ??= new List<TourItemInputViewModel>();

            while (model.Items.Count < 2)
            {
                model.Items.Add(new TourItemInputViewModel
                {
                    OrderIndex = model.Items.Count + 1
                });
            }
        }

        public class FoodStallOptionViewModel
        {
            public int Id { get; set; }
            public string Label { get; set; } = "";
            public string SearchText { get; set; } = "";
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}