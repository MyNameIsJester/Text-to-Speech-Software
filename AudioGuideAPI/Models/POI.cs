namespace AudioGuideAPI.Models
{
    public class POI
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Radius { get; set; }

        public string Description { get; set; } = string.Empty;

        public string AudioUrl { get; set; } = string.Empty;
    }
}