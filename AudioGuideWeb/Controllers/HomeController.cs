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

        public async Task<IActionResult> Index(
            string lang = "vi",
            string? voiceLang = null,
            int? tourId = null,
            bool startTour = false)
        {
            voiceLang ??= lang;

            var vm = new HomeIndexViewModel
            {
                CurrentLanguage = voiceLang,
                SelectedTourId = tourId,
                StartTourRequested = startTour,
                IsTourMode = startTour && tourId.HasValue
            };

            try
            {
                vm.Languages = await _apiService.GetLanguagesAsync();

                // Quan trọng: dữ liệu quán dùng để phát TTS lấy theo voiceLang
                vm.FoodStalls = await _apiService.GetFoodStallsAsync(voiceLang);

                if (vm.IsTourMode && tourId.HasValue)
                {
                    // Quan trọng: tour + stops dùng để phát TTS lấy theo voiceLang
                    vm.SelectedTour = await _apiService.GetTourByIdAsync(tourId.Value, voiceLang);

                    if (vm.SelectedTour == null)
                    {
                        vm.IsTourMode = false;
                        vm.StartTourRequested = false;
                        vm.SelectedTourId = null;
                        vm.ErrorMessage = lang == "en"
                            ? "The selected tour was not found or is currently unavailable."
                            : "Không tìm thấy tour được chọn hoặc tour hiện không khả dụng.";
                    }
                }
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = lang == "en"
                    ? $"Could not load data from API: {ex.Message}"
                    : $"Không thể tải dữ liệu từ API: {ex.Message}";
            }

            return View(vm);
        }
    }
}