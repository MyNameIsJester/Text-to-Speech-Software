namespace TravelTracker.Model;

public class LanguageOption
{
    public string FlagIcon { get; set; }
    public string LanguageCode { get; set; }
    public string IntroText { get; set; }
    public string DisplayName { get; set; }
    public List<FoodStall> Stalls { get; set; }
}