namespace TravelTracker.Model;

public class TourItem
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int FoodStallId { get; set; }

    public int OrderIndex { get; set; }

    public FoodStall FoodStall { get; set; }
}