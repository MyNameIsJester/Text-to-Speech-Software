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
        public int DemoLogs { get; set; }

        public int ViLogs { get; set; }
        public int EnLogs { get; set; }

        public int CompletedLogs { get; set; }
        public int StoppedLogs { get; set; }
        public int InterruptedLogs { get; set; }

        public int TotalActualListeningSeconds { get; set; }

        public List<RecentPlaybackLogViewModel> RecentPlaybackLogs { get; set; } = new();
        public List<TopFoodStallViewModel> TopFoodStalls { get; set; } = new();

        public int TotalWebVisits { get; set; }
        public int ActiveWebUsers { get; set; }
    }

    public class RecentPlaybackLogViewModel
    {
        public int Id { get; set; }
        public int FoodStallId { get; set; }
        public string FoodStallName { get; set; } = "";
        public string FoodStallAddress { get; set; } = "";
        public string LanguageCode { get; set; } = "";
        public string TriggerType { get; set; } = "";
        public string Status { get; set; } = "";

        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public int ActualDurationSeconds { get; set; }
    }

    public class TopFoodStallViewModel
    {
        public int FoodStallId { get; set; }
        public string FoodStallName { get; set; } = "";
        public string FoodStallAddress { get; set; } = "";
        public int PlaybackCount { get; set; }

        public int TotalActualDurationSeconds { get; set; }
    }
}