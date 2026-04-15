namespace TravelTracker.Model;

public class TourTranslation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Language Language { get; set; }
}