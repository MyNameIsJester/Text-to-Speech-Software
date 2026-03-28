using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPOIData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "POIs",
                columns: new[] { "Id", "AudioUrl", "Description", "Latitude", "Longitude", "Name", "Radius" },
                values: new object[,]
                {
                    { 1, "audio/vinhkhanh.mp3", "Khu phố nổi tiếng với nhiều món ăn đêm.", 10.755100000000001, 106.7038, "Phố ẩm thực Vĩnh Khánh", 50.0 },
                    { 2, "audio/oc.mp3", "Điểm dừng chân gợi ý để trải nghiệm món ốc.", 10.754799999999999, 106.7041, "Quán ốc nổi bật", 30.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
