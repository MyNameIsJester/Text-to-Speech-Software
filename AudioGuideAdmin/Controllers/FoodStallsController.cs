using AudioGuideAdmin.Models;
using AudioGuideAdmin.Services;
using AudioGuideAdmin.ViewModels.FoodStalls;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin,FoodStallOwner")]
    public class FoodStallsController : Controller
    {
        private readonly AdminFoodStallApiService _foodStallApiService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodStallsController(
            AdminFoodStallApiService foodStallApiService,
            UserManager<ApplicationUser> userManager)
        {
            _foodStallApiService = foodStallApiService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchTerm, string statusFilter = "all")
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var foodStalls = await _foodStallApiService.GetFoodStallsAsync();
            var query = foodStalls.AsEnumerable();

            if (!isAdmin)
            {
                query = query.Where(x => x.OwnerUserId == currentUserId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim().ToLowerInvariant();

                query = query.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.Address) && x.Address.ToLowerInvariant().Contains(keyword)) ||
                    x.Translations.Any(t => !string.IsNullOrWhiteSpace(t.Name) && t.Name.ToLowerInvariant().Contains(keyword))
                );
            }

            if (statusFilter == "active")
            {
                query = query.Where(x => x.IsActive);
            }
            else if (statusFilter == "inactive")
            {
                query = query.Where(x => !x.IsActive);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.StatusFilter = statusFilter;

            return View(query.OrderBy(x => x.Id).ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateOwnerOptionsAsync();

            var model = new FoodStall
            {
                Radius = 35,
                Priority = 0,
                IsActive = true
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodStall model)
        {
            await PopulateOwnerOptionsAsync(model.OwnerUserId);

            if (!await IsValidOwnerSelectionAsync(model.OwnerUserId))
            {
                ModelState.AddModelError(nameof(model.OwnerUserId), "Selected owner is invalid.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var created = await _foodStallApiService.CreateFoodStallAsync(model);

            return RedirectToAction(nameof(EditTranslations), new { id = created.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var foodStall = await GetAccessibleFoodStallByIdAsync(id);

            if (foodStall == null)
                return NotFound();

            if (User.IsInRole("Admin"))
            {
                await PopulateOwnerOptionsAsync(foodStall.OwnerUserId);
            }

            return View(foodStall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FoodStall model)
        {
            if (id != model.Id)
                return NotFound();

            var existing = await GetAccessibleFoodStallByIdAsync(id);

            if (existing == null)
                return NotFound();

            if (!User.IsInRole("Admin"))
            {
                model.OwnerUserId = existing.OwnerUserId;
            }
            else
            {
                await PopulateOwnerOptionsAsync(model.OwnerUserId);

                if (!await IsValidOwnerSelectionAsync(model.OwnerUserId))
                {
                    ModelState.AddModelError(nameof(model.OwnerUserId), "Selected owner is invalid.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            await _foodStallApiService.UpdateFoodStallAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var foodStall = await _foodStallApiService.GetFoodStallByIdAsync(id);

            if (foodStall == null)
                return NotFound();

            return View(foodStall);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodStall = await _foodStallApiService.GetFoodStallByIdAsync(id);

            if (foodStall == null)
                return NotFound();

            await _foodStallApiService.DeleteFoodStallAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditTranslations(int id)
        {
            var foodStall = await GetAccessibleFoodStallByIdAsync(id);

            if (foodStall == null)
                return NotFound();

            var vm = await _foodStallApiService.GetTranslationsEditorAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTranslations(int id, FoodStallTranslationsEditViewModel model)
        {
            if (id != model.FoodStallId)
                return NotFound();

            var foodStall = await GetAccessibleFoodStallByIdAsync(id);

            if (foodStall == null)
                return NotFound();

            await _foodStallApiService.SaveTranslationsAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        private async Task<FoodStall?> GetAccessibleFoodStallByIdAsync(int id)
        {
            var foodStall = await _foodStallApiService.GetFoodStallByIdAsync(id);

            if (foodStall == null)
                return null;

            return await CanAccessFoodStallAsync(foodStall) ? foodStall : null;
        }

        private async Task<bool> CanAccessFoodStallAsync(FoodStall foodStall)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUserId = _userManager.GetUserId(User);
            return !string.IsNullOrWhiteSpace(currentUserId) &&
                   foodStall.OwnerUserId == currentUserId;
        }

        private async Task PopulateOwnerOptionsAsync(string? selectedOwnerUserId = null)
        {
            var owners = await _userManager.GetUsersInRoleAsync("FoodStallOwner");

            var lookupItems = owners
                .OrderBy(x => x.Email)
                .Select(x => new
                {
                    Id = x.Id,
                    DisplayText = string.IsNullOrWhiteSpace(x.FullName)
                        ? (x.Email ?? x.UserName ?? x.Id)
                        : $"{x.FullName} ({x.Email})"
                })
                .ToList();

            var selectedDisplayText = lookupItems
                .FirstOrDefault(x => x.Id == selectedOwnerUserId)
                ?.DisplayText ?? "";

            ViewBag.OwnerLookupItems = lookupItems;
            ViewBag.OwnerLookupJson = JsonSerializer.Serialize(lookupItems);
            ViewBag.SelectedOwnerDisplayText = selectedDisplayText;
        }

        private async Task<bool> IsValidOwnerSelectionAsync(string? ownerUserId)
        {
            if (string.IsNullOrWhiteSpace(ownerUserId))
                return true;

            var user = await _userManager.FindByIdAsync(ownerUserId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "FoodStallOwner");
        }
    }
}