using System.ComponentModel.DataAnnotations;

namespace AudioGuideAdmin.ViewModels.Tours
{
    public class TourEditViewModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        public TourTranslationInputViewModel Vietnamese { get; set; } = new();

        public TourTranslationInputViewModel English { get; set; } = new();

        public List<TourItemInputViewModel> Items { get; set; } = new();
    }

    public class TourTranslationInputViewModel
    {
        public int? TourTranslationId { get; set; }

        public int LanguageId { get; set; }

        public string LanguageCode { get; set; } = "";

        public string DisplayName { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }
    }

    public class TourItemInputViewModel
    {
        public int? TourItemId { get; set; }

        public int FoodStallId { get; set; }

        public string FoodStallLabel { get; set; } = "";

        public int OrderIndex { get; set; }
    }
}