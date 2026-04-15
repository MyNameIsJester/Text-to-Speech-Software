using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class TourTranslation
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}