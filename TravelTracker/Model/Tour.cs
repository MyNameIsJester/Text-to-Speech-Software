namespace TravelTracker.Model;

public class Tour
{
    public int Id { get; set; }
    public bool IsActive { get; set; }

    public List<TourTranslation> Translations { get; set; } = new();

    public List<TourItem> TourItems { get; set; } = new();

    public string Name { get; set; }
    public string Description { get; set; }

    public int TotalStops => TourItems?.Count ?? 0;
}
