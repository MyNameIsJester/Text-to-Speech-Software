namespace AudioGuideWeb.Models
{
    public class LanguageViewModel
    {
        public string LanguageCode { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? FlagIcon { get; set; }
        public string? IntroText { get; set; }
        public bool IsDefault { get; set; }
    }
}