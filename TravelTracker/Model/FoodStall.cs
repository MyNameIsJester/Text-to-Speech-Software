using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TravelTracker.Model
{
    public class FoodStall
    {
        // Định nghĩa các thuộc tính
        public string Name { get; set; }       
        public string Address { get; set; }    
        public string Specialty { get; set; }   
        public string ImageUrl { get; set; }    
        public string Description { get; set; } 
        public string PriceRange { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private string _distanceText;
        public string DistanceText
        {
            get => _distanceText;
            set
            {
                _distanceText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}