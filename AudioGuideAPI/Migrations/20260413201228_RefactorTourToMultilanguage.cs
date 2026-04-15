using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTourToMultilanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tours");

            migrationBuilder.CreateTable(
                name: "TourTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourTranslations_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslations_LanguageId",
                table: "TourTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TourTranslations_TourId_LanguageId",
                table: "TourTranslations",
                columns: new[] { "TourId", "LanguageId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourTranslations");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tours",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tours",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
