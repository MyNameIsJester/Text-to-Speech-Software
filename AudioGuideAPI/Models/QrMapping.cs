using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class QrMapping
    {
        public int Id { get; set; }

        public int FoodStallId { get; set; }
        public FoodStall FoodStall { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string CodeValue { get; set; } = "";

        public bool IsActive { get; set; } = true;
    }
}