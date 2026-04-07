namespace AudioGuideAPI.DTOs
{
    public class LanguageDto
    {
        public string LanguageCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? FlagIcon { get; set; }
        public string? IntroText { get; set; }
        public bool IsDefault { get; set; }
    }
}