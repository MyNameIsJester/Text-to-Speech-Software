using AudioGuideWeb.Models;
using AudioGuideWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideWeb.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApiService _apiService;

        public ToursController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string lang = "vi")
        {
            var model = new List<TourViewModel>();

            try
            {
                model = await _apiService.GetToursAsync(lang);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Không thể tải danh sách tour: {ex.Message}";
            }

            ViewBag.CurrentLanguage = lang;
            return View(model);
        }

        public async Task<IActionResult> Details(int id, string lang = "vi")
        {
            TourViewModel? model = null;

            try
            {
                model = await _apiService.GetTourByIdAsync(id, lang);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Không thể tải chi tiết tour: {ex.Message}";
            }

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.CurrentLanguage = lang;
            return View(model);
        }
    }
}