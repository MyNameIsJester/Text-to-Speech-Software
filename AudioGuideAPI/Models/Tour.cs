namespace AudioGuideAPI.Models
{
    public class Tour
    {
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TourTranslation> Translations { get; set; } = new List<TourTranslation>();

        public ICollection<TourItem> TourItems { get; set; } = new List<TourItem>();
    }
}