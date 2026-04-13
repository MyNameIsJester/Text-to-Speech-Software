using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class FoodStallTranslation
    {
        public int Id { get; set; }

        public int FoodStallId { get; set; }
        public FoodStall FoodStall { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? TtsScript { get; set; }

        [MaxLength(500)]
        public string? Specialty { get; set; }

        [MaxLength(255)]
        public string? AudioUrl { get; set; }
    }
}