namespace AudioGuideWeb.Models
{
    public class TourViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LanguageCode { get; set; }

        public int StopCount { get; set; }

        public List<TourStopViewModel> Stops { get; set; } = new();
    }

    public class TourStopViewModel
    {
        public int FoodStallId { get; set; }
        public int OrderIndex { get; set; }

        public string? Name { get; set; }
        public string? Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }

        public string? MapLink { get; set; }
    }
}