using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateTimeCOlumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTimeUtc",
                table: "Termins",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndTimeUtc",
                table: "Termins",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "StartTimeUtc",
                table: "EventSchedule",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndTimeUtc",
                table: "EventSchedule",
                newName: "EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Termins",
                newName: "StartTimeUtc");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Termins",
                newName: "EndTimeUtc");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "EventSchedule",
                newName: "StartTimeUtc");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "EventSchedule",
                newName: "EndTimeUtc");
        }
    }
}
