using AudioGuideAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<POI> POIs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);         //OnModelCreating dùng để cấu hình model (Bảng, quan hệ, dữ liệu,...), EF sẽ đọc đoạn này khi tạo Migration

            modelBuilder.Entity<POI>().HasData(         //HasData "Khi tạo database, insert sẵn các dòng này vào bảng POIs"
                new POI
                {
                    Id = 1,
                    Name = "Phố ẩm thực Vĩnh Khánh",
                    Latitude = 10.7551,
                    Longitude = 106.7038,
                    Radius = 50,
                    Description = "Khu phố nổi tiếng với nhiều món ăn đêm.",
                    AudioUrl = "audio/vinhkhanh.mp3"
                },
                new POI
                {
                    Id = 2,
                    Name = "Quán ốc nổi bật",
                    Latitude = 10.7548,
                    Longitude = 106.7041,
                    Radius = 30,
                    Description = "Điểm dừng chân gợi ý để trải nghiệm món ốc.",
                    AudioUrl = "audio/oc.mp3"
                }
            );
        }
    }
}