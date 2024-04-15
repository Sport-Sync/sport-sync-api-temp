using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCanceledToMatchApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Canceled",
                table: "MatchApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Canceled",
                table: "MatchApplications");
        }
    }
}
