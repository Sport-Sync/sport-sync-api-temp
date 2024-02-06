using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateFriendshipRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSchedule_Events_EventId",
                table: "EventSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_EventSchedule_ScheduleId",
                table: "Termins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSchedule",
                table: "EventSchedule");

            migrationBuilder.RenameTable(
                name: "EventSchedule",
                newName: "EventSchedules");

            migrationBuilder.RenameIndex(
                name: "IX_EventSchedule_EventId",
                table: "EventSchedules",
                newName: "IX_EventSchedules_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSchedules",
                table: "EventSchedules",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FriendshipRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Rejected = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendshipRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendshipRequests_Users_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FriendshipRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipRequests_FriendId",
                table: "FriendshipRequests",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipRequests_UserId",
                table: "FriendshipRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSchedules_Events_EventId",
                table: "EventSchedules",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_EventSchedules_ScheduleId",
                table: "Termins",
                column: "ScheduleId",
                principalTable: "EventSchedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSchedules_Events_EventId",
                table: "EventSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Termins_EventSchedules_ScheduleId",
                table: "Termins");

            migrationBuilder.DropTable(
                name: "FriendshipRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSchedules",
                table: "EventSchedules");

            migrationBuilder.RenameTable(
                name: "EventSchedules",
                newName: "EventSchedule");

            migrationBuilder.RenameIndex(
                name: "IX_EventSchedules_EventId",
                table: "EventSchedule",
                newName: "IX_EventSchedule_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSchedule",
                table: "EventSchedule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSchedule_Events_EventId",
                table: "EventSchedule",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Termins_EventSchedule_ScheduleId",
                table: "Termins",
                column: "ScheduleId",
                principalTable: "EventSchedule",
                principalColumn: "Id");
        }
    }
}
