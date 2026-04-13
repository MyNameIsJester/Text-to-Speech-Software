namespace AudioGuideAdmin.ViewModels.FoodStalls
{
    public class FoodStallTranslationsEditViewModel
    {
        public int FoodStallId { get; set; }
        public string StallAddress { get; set; } = "";

        public TranslationInputViewModel Vietnamese { get; set; } = new();
        public TranslationInputViewModel English { get; set; } = new();
    }

    public class TranslationInputViewModel
    {
        public int? TranslationId { get; set; }
        public int LanguageId { get; set; }

        public string LanguageCode { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? TtsScript { get; set; }
        public string? Specialty { get; set; }
        public string? AudioUrl { get; set; }
    }
}