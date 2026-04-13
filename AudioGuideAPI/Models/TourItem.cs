namespace AudioGuideAPI.Models
{
    public class TourItem
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;

        public int FoodStallId { get; set; }
        public FoodStall FoodStall { get; set; } = null!;

        public int OrderIndex { get; set; }
    }
}