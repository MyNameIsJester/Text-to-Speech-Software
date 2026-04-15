namespace AudioGuideAdmin.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalFoodStalls { get; set; }
        public int ActiveFoodStalls { get; set; }
        public int TotalPlaybackLogs { get; set; }
        public int TotalQrMappings { get; set; }
        public int TotalTours { get; set; }

        public int GpsLogs { get; set; }
        public int QrLogs { get; set; }
        public int ViLogs { get; set; }
        public int EnLogs { get; set; }

        public List<RecentPlaybackLogViewModel> RecentPlaybackLogs { get; set; } = new();
        public List<TopFoodStallViewModel> TopFoodStalls { get; set; } = new();
    }

    public class RecentPlaybackLogViewModel
    {
        public int Id { get; set; }
        public int FoodStallId { get; set; }
        public string FoodStallAddress { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public string TriggerType { get; set; } = "";
        public DateTime PlayedAt { get; set; }
        public int DurationSeconds { get; set; }
    }

    public class TopFoodStallViewModel
    {
        public int FoodStallId { get; set; }
        public string FoodStallAddress { get; set; } = "";
        public int PlaybackCount { get; set; }
    }
}