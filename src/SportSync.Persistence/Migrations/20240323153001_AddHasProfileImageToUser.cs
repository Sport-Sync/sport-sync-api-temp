using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHasProfileImageToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasProfileImage",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasProfileImage",
                table: "Users");
        }
    }
}
