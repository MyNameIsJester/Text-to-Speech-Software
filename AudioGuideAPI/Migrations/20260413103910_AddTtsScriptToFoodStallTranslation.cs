using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTtsScriptToFoodStallTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TtsScript",
                table: "FoodStallTranslations",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TtsScript",
                table: "FoodStallTranslations");
        }
    }
}
