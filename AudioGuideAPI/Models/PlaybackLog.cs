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
        public string TriggerType { get; set; } = ""; // GPS / DEMO / TOUR

        [MaxLength(20)]
        public string Status { get; set; } = "Started"; // Started / Completed / Stopped / Interrupted

        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public int ActualDurationSeconds { get; set; }
    }
}