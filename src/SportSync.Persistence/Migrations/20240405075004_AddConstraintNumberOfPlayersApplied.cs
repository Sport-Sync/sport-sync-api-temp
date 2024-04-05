using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintNumberOfPlayersApplied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchAnnouncements_MatchId",
                table: "MatchAnnouncements");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "MatchAnnouncements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                table: "MatchAnnouncements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MatchAnnouncements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPlayersAccepted",
                table: "MatchAnnouncements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPlayersLimit",
                table: "MatchAnnouncements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MatchAnnouncements_MatchId",
                table: "MatchAnnouncements",
                column: "MatchId",
                unique: true);

            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements ADD CONSTRAINT CHK_NumberOfPlayersAccepted CHECK (NumberOfPlayersAccepted <= NumberOfPlayersLimit)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements DROP CONSTRAINT CHK_NumberOfPlayersAccepted");

            migrationBuilder.DropIndex(
                name: "IX_MatchAnnouncements_MatchId",
                table: "MatchAnnouncements");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "MatchAnnouncements");

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                table: "MatchAnnouncements");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MatchAnnouncements");

            migrationBuilder.DropColumn(
                name: "NumberOfPlayersAccepted",
                table: "MatchAnnouncements");

            migrationBuilder.DropColumn(
                name: "NumberOfPlayersLimit",
                table: "MatchAnnouncements");

            migrationBuilder.CreateIndex(
                name: "IX_MatchAnnouncements_MatchId",
                table: "MatchAnnouncements",
                column: "MatchId");
        }
    }
}
