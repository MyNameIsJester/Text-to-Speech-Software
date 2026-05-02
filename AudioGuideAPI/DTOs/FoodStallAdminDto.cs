namespace AudioGuideAPI.DTOs
{
    public class FoodStallAdminDto
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public int Priority { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? PriceRange { get; set; }
        public string? MapLink { get; set; }
        public bool IsActive { get; set; }
        public string? OwnerUserId { get; set; }

        public List<FoodStallAdminTranslationDto> Translations { get; set; } = new();
    }

    public class FoodStallAdminTranslationDto
    {
        public int? TranslationId { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Specialty { get; set; }
        public string? AudioUrl { get; set; }
    }
}