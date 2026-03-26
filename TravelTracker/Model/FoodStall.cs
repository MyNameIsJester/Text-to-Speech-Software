using System;
using System.Collections.Generic;
using System.Text;

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
    }
}