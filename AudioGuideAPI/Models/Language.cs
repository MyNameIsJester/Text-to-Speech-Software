using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class Language
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = null!;   // vi, en, ja

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = null!;    // Tiếng Việt, English

        [MaxLength(255)]
        public string? FlagIcon { get; set; }               // /flags/vn.png

        [MaxLength(500)]
        public string? IntroText { get; set; }

        public bool IsDefault { get; set; } = false;

        public ICollection<FoodStallTranslation> FoodStallTranslations { get; set; }
            = new List<FoodStallTranslation>();
    }
}