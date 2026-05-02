namespace AudioGuideWeb.Models
{
    public class HomeIndexViewModel
    {
        public string CurrentLanguage { get; set; } = "vi";
        public List<LanguageViewModel> Languages { get; set; } = new();
        public List<FoodStallViewModel> FoodStalls { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public bool IsTourMode { get; set; }
        public bool StartTourRequested { get; set; }

        public int? SelectedTourId { get; set; }
        public TourViewModel? SelectedTour { get; set; }
    }
}