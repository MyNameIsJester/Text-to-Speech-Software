namespace AudioGuideAPI.DTOs
{
    public class FoodStallMobileDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Specialty { get; set; }
        public string? PriceRange { get; set; }
        public string? ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public string? Description { get; set; }
        public string? AudioUrl { get; set; }
        public int Priority { get; set; }
        public string? MapLink { get; set; }
        public string? LanguageCode { get; set; }
    }
}