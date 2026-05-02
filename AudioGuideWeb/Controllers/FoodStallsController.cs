using AudioGuideWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideWeb.Controllers
{
    public class FoodStallsController : Controller
    {
        private readonly ApiService _apiService;

        public FoodStallsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string lang = "vi")
        {
            var foodStalls = await _apiService.GetFoodStallsAsync(lang);
            return View(foodStalls);
        }

        public async Task<IActionResult> Details(int id, string lang = "vi")
        {
            var stall = await _apiService.GetFoodStallByIdAsync(id, lang);

            if (stall == null)
            {
                return NotFound();
            }

            return View(stall);
        }
    }
}