namespace AudioGuideAPI.DTOs
{
    public class FoodStallAdminUpsertDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public int Priority { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? PriceRange { get; set; }
        public string? MapLink { get; set; }
        public bool IsActive { get; set; }
        public string? OwnerUserId { get; set; }
    }
}