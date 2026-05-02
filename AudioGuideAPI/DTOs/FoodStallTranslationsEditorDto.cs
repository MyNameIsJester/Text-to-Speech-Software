namespace AudioGuideAPI.DTOs
{
    public class FoodStallTranslationsEditorDto
    {
        public int FoodStallId { get; set; }
        public string StallAddress { get; set; } = "";

        public FoodStallTranslationInputDto Vietnamese { get; set; } = new();
        public FoodStallTranslationInputDto English { get; set; } = new();
    }

    public class FoodStallTranslationInputDto
    {
        public int? TranslationId { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Specialty { get; set; }
        public string? AudioUrl { get; set; }
    }
}