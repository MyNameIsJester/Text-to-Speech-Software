using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class RedesignPlaybackLogsLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayedAt",
                table: "PlaybackLogs",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "DurationSeconds",
                table: "PlaybackLogs",
                newName: "EstimatedDurationSeconds");

            migrationBuilder.AddColumn<int>(
                name: "ActualDurationSeconds",
                table: "PlaybackLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "PlaybackLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PlaybackLogs",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDurationSeconds",
                table: "PlaybackLogs");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "PlaybackLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PlaybackLogs");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "PlaybackLogs",
                newName: "PlayedAt");

            migrationBuilder.RenameColumn(
                name: "EstimatedDurationSeconds",
                table: "PlaybackLogs",
                newName: "DurationSeconds");
        }
    }
}
