using AudioGuideWeb.Models;
using AudioGuideWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string lang = "vi", int? tourId = null, bool startTour = false)
        {
            var vm = new HomeIndexViewModel
            {
                CurrentLanguage = lang,
                SelectedTourId = tourId,
                StartTourRequested = startTour,
                IsTourMode = startTour && tourId.HasValue
            };

            try
            {
                vm.Languages = await _apiService.GetLanguagesAsync();
                vm.FoodStalls = await _apiService.GetFoodStallsAsync(lang);

                if (vm.IsTourMode && tourId.HasValue)
                {
                    vm.SelectedTour = await _apiService.GetTourByIdAsync(tourId.Value, lang);

                    if (vm.SelectedTour == null)
                    {
                        vm.IsTourMode = false;
                        vm.StartTourRequested = false;
                        vm.SelectedTourId = null;
                        vm.ErrorMessage = "Không tìm thấy tour được chọn hoặc tour hiện không khả dụng.";
                    }
                }
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = $"Không thể tải dữ liệu từ API: {ex.Message}";
            }

            return View(vm);
        }
    }
}