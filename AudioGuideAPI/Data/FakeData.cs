using AudioGuideAPI.Models;

namespace AudioGuideAPI.Data
{
    public static class FakeData
    {
        public static List<POI> GetPOIs()   //Hàm trả về danh sách các quán ăn
        {
            return new List<POI>      //Danh sách nhiều poi
            {
                new POI    //Tạo từng quán ăn
                {
                    Id = 1,
                    Name = "Quán Ốc A",
                    Latitude = 10.762622,
                    Longitude = 106.660172,
                    Radius = 50,
                    Description = "Quán ốc nổi tiếng tại Vĩnh Khánh",
                    AudioUrl = "audio/ocA.mp3"
                },
                new POI
                {
                    Id = 2,
                    Name = "Quán Bún Bò B",
                    Latitude = 10.762800,
                    Longitude = 106.660300,
                    Radius = 50,
                    Description = "Bún bò Huế đậm vị",
                    AudioUrl = "audio/bunboB.mp3"
                }
            };
        }
    }
}