using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitFoodStallI18n : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodStalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Radius = table.Column<double>(type: "REAL", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PriceRange = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    MapLink = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FlagIcon = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IntroText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodStallTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodStallId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AudioUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodStallTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodStallTranslations_FoodStalls_FoodStallId",
                        column: x => x.FoodStallId,
                        principalTable: "FoodStalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodStallTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodStallTranslations_FoodStallId_LanguageId",
                table: "FoodStallTranslations",
                columns: new[] { "FoodStallId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodStallTranslations_LanguageId",
                table: "FoodStallTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LanguageCode",
                table: "Languages",
                column: "LanguageCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodStallTranslations");

            migrationBuilder.DropTable(
                name: "FoodStalls");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
