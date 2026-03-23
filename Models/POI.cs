namespace AudioGuideAPI.Models    //Một địa chỉ giúp biết class này nằm ở đâu 
{
    public class POI    //định nghĩa một POI - mỗi quán ăn gồm có những thuộc tính gì?
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }    //App sẽ dùng GPS để so sánh với tọa độ này

        public double Longitude { get; set; }   //App sẽ dùng GPS để so sánh với tọa độ này

        public double Radius { get; set; }      //Khi user vào vùng bán kính sẽ phát audio

        public string Description { get; set; }

        public string AudioUrl { get; set; }  
    }
}