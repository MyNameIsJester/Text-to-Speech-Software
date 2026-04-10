using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravelTracker.Model
{
    public class FoodStall : INotifyPropertyChanged
    {
        // Định nghĩa các thuộc tính
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Specialty { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string PriceRange { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; }
        public string AudioUrl { get; set; }
        public int Priority { get; set; }
        public string MapLink { get; set; }
        public string LanguageCode { get; set; }


        private string _distanceText;
        public string DistanceText
        {
            get => _distanceText;
            set
            {
                if (_distanceText != value)
                {
                    _distanceText = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FavoriteButtonText));
                    OnPropertyChanged(nameof(FavoriteButtonColor));
                }
            }
        }

        public string FavoriteButtonText => IsFavorite ? "❤️" : "🤍";
        public Color FavoriteButtonColor => IsFavorite ? Colors.Transparent : Colors.Transparent;
    }
}