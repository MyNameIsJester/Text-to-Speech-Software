using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddQrMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QrMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodStallId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodeValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrMappings_FoodStalls_FoodStallId",
                        column: x => x.FoodStallId,
                        principalTable: "FoodStalls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QrMappings_CodeValue",
                table: "QrMappings",
                column: "CodeValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QrMappings_FoodStallId",
                table: "QrMappings",
                column: "FoodStallId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QrMappings");
        }
    }
}
