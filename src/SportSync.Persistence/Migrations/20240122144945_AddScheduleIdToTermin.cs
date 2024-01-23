using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIdToTermin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete dbo.Players");
            migrationBuilder.Sql("Delete dbo.Termins");
            migrationBuilder.Sql("Delete dbo.EventSchedule");
            migrationBuilder.Sql("Delete dbo.EventMembers");
            migrationBuilder.Sql("Delete dbo.Events");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "Termins",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Termins_ScheduleId",
                table: "Termins",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_EventSchedule_ScheduleId",
                table: "Termins",
                column: "ScheduleId",
                principalTable: "EventSchedule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termins_EventSchedule_ScheduleId",
                table: "Termins");

            migrationBuilder.DropIndex(
                name: "IX_Termins_ScheduleId",
                table: "Termins");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Termins");
        }
    }
}
