using AudioGuideAdmin.Services;
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
        private readonly AdminTourApiService _tourApi;

        public ToursController(AppDbContext context, AdminTourApiService tourApi)
        {
            _context = context;
            _tourApi = tourApi;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var tours = await _tourApi.GetToursAsync();
            var query = tours.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLowerInvariant();

                query = query.Where(x =>
                    x.Translations.Any(t =>
                        (!string.IsNullOrWhiteSpace(t.Name) && t.Name.ToLowerInvariant().Contains(keyword)) ||
                        (!string.IsNullOrWhiteSpace(t.Description) && t.Description.ToLowerInvariant().Contains(keyword))
                    ));
            }

            ViewBag.Search = search ?? "";

            return View(query.OrderBy(x => x.Id).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _tourApi.GetTourByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            await HydrateTourFoodStallDetailsAsync(tour);

            return View(tour);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadFoodStallOptions();

            var vm = new TourEditViewModel
            {
                IsActive = true,
                Vietnamese = new TourTranslationInputViewModel
                {
                    LanguageCode = "vi",
                    DisplayName = "Tiếng Việt"
                },
                English = new TourTranslationInputViewModel
                {
                    LanguageCode = "en",
                    DisplayName = "English"
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
            PopulateLanguageMetadata(model);

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

            await _tourApi.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var tour = await _tourApi.GetTourByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            await LoadFoodStallOptions();
            var optionMap = await GetFoodStallOptionMapAsync();

            var viTranslation = tour.Translations.FirstOrDefault(t => t.Language?.LanguageCode == "vi");
            var enTranslation = tour.Translations.FirstOrDefault(t => t.Language?.LanguageCode == "en");

            var vm = new TourEditViewModel
            {
                Id = tour.Id,
                IsActive = tour.IsActive,

                Vietnamese = new TourTranslationInputViewModel
                {
                    LanguageCode = "vi",
                    DisplayName = "Tiếng Việt",
                    Name = viTranslation?.Name ?? "",
                    Description = viTranslation?.Description
                },

                English = new TourTranslationInputViewModel
                {
                    LanguageCode = "en",
                    DisplayName = "English",
                    Name = enTranslation?.Name ?? "",
                    Description = enTranslation?.Description
                },

                Items = tour.TourItems
                    .OrderBy(x => x.OrderIndex)
                    .Select(x => new TourItemInputViewModel
                    {
                        TourItemId = x.Id,
                        FoodStallId = x.FoodStallId,
                        FoodStallLabel = optionMap.TryGetValue(x.FoodStallId, out var label) ? label : $"{x.FoodStallId}",
                        OrderIndex = x.OrderIndex
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

            PopulateLanguageMetadata(model);

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

            var existing = await _tourApi.GetTourByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            await _tourApi.UpdateAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _tourApi.GetTourByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            await HydrateTourFoodStallDetailsAsync(tour);

            return View(tour);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = await _tourApi.GetTourByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            await _tourApi.DeleteAsync(id);

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

        private static void PopulateLanguageMetadata(TourEditViewModel model)
        {
            model.Vietnamese.LanguageCode = "vi";
            model.Vietnamese.DisplayName = "Tiếng Việt";

            model.English.LanguageCode = "en";
            model.English.DisplayName = "English";
        }

        private async Task<Dictionary<int, string>> GetFoodStallOptionMapAsync()
        {
            var foodStalls = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return foodStalls.ToDictionary(
                x => x.Id,
                x =>
                {
                    var viName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "vi")?.Name;
                    var enName = x.Translations.FirstOrDefault(t => t.Language.LanguageCode == "en")?.Name;

                    var displayName = !string.IsNullOrWhiteSpace(viName)
                        ? viName
                        : !string.IsNullOrWhiteSpace(enName)
                            ? enName
                            : (x.Address ?? $"Food Stall {x.Id}");

                    return $"{x.Id} - {displayName}";
                });
        }

        private async Task HydrateTourFoodStallDetailsAsync(Tour tour)
        {
            if (tour.TourItems == null || !tour.TourItems.Any())
            {
                return;
            }

            var foodStallIds = tour.TourItems
                .Select(x => x.FoodStallId)
                .Distinct()
                .ToList();

            var foodStalls = await _context.FoodStalls
                .Include(x => x.Translations)
                    .ThenInclude(t => t.Language)
                .Where(x => foodStallIds.Contains(x.Id))
                .ToListAsync();

            var foodStallMap = foodStalls.ToDictionary(x => x.Id, x => x);

            foreach (var item in tour.TourItems)
            {
                if (foodStallMap.TryGetValue(item.FoodStallId, out var stall))
                {
                    item.FoodStall = stall;
                }
            }
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