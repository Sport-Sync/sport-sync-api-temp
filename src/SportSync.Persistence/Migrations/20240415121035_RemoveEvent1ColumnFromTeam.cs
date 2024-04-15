using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEvent1ColumnFromTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Events_EventId1",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_EventId1",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "EventId1",
                table: "Teams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId1",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_EventId1",
                table: "Teams",
                column: "EventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Events_EventId1",
                table: "Teams",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
