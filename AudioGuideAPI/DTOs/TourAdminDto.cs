namespace AudioGuideAPI.DTOs
{
    public class TourAdminDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public string VietnameseName { get; set; } = "";
        public string? VietnameseDescription { get; set; }

        public string EnglishName { get; set; } = "";
        public string? EnglishDescription { get; set; }

        public List<TourItemDto> Items { get; set; } = new();
    }

    public class TourItemDto
    {
        public int FoodStallId { get; set; }
        public int OrderIndex { get; set; }
    }
}