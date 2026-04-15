using System.ComponentModel.DataAnnotations;

namespace AudioGuideAPI.Models
{
    public class PlaybackLog
    {
        public int Id { get; set; }

        public int FoodStallId { get; set; }
        public FoodStall FoodStall { get; set; } = null!;

        [MaxLength(10)]
        public string LanguageCode { get; set; } = "";

        [MaxLength(20)]
        public string TriggerType { get; set; } = ""; // GPS / QR

        public DateTime PlayedAt { get; set; }

        public int DurationSeconds { get; set; }
    }
}