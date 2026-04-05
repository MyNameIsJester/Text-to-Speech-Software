using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class FoodStall
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        // bán kính kích hoạt audio (đơn vị mét)
        public double Radius { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? PriceRange { get; set; }

        public int Priority { get; set; } = 0;

        [MaxLength(500)]
        public string? MapLink { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<FoodStallTranslation> Translations { get; set; }
            = new List<FoodStallTranslation>();
    }
}