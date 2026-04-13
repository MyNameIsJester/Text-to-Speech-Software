using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    public class QrMappingsController : Controller
    {
        private readonly AppDbContext _context;

        public QrMappingsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var qrMappings = await _context.QrMappings
                .Include(x => x.FoodStall)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return View(qrMappings);
        }

        public async Task<IActionResult> Create()
        {
            await LoadFoodStallOptions();
            return View(new QrMapping { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QrMapping model)
        {
            if (!ModelState.IsValid)
            {
                await LoadFoodStallOptions(model.FoodStallId);
                return View(model);
            }

            model.CodeValue = model.CodeValue?.Trim() ?? "";

            _context.QrMappings.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var qrMapping = await _context.QrMappings.FindAsync(id);

            if (qrMapping == null)
                return NotFound();

            await LoadFoodStallOptions(qrMapping.FoodStallId);
            return View(qrMapping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QrMapping model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadFoodStallOptions(model.FoodStallId);
                return View(model);
            }

            var qrMapping = await _context.QrMappings.FindAsync(id);

            if (qrMapping == null)
                return NotFound();

            qrMapping.FoodStallId = model.FoodStallId;
            qrMapping.CodeValue = model.CodeValue?.Trim() ?? "";
            qrMapping.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var qrMapping = await _context.QrMappings
                .Include(x => x.FoodStall)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (qrMapping == null)
                return NotFound();

            return View(qrMapping);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var qrMapping = await _context.QrMappings.FindAsync(id);

            if (qrMapping == null)
                return NotFound();

            _context.QrMappings.Remove(qrMapping);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFoodStallOptions(int? selectedFoodStallId = null)
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
                    SearchText = $"{displayName} {x.Address}".Trim()
                };
            }).ToList();

            string selectedLabel = "";
            if (selectedFoodStallId.HasValue)
            {
                selectedLabel = options.FirstOrDefault(x => x.Id == selectedFoodStallId.Value)?.Label ?? "";
            }

            ViewBag.FoodStallOptions = options;
            ViewBag.SelectedFoodStallLabel = selectedLabel;
        }

        public class FoodStallOptionViewModel
        {
            public int Id { get; set; }
            public string Label { get; set; } = "";
            public string SearchText { get; set; } = "";
        }
    }
}