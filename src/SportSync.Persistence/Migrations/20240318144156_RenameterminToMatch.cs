using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameterminToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM TerminApplications");
            migrationBuilder.Sql("DELETE FROM TerminAnnouncements");
            migrationBuilder.Sql("DELETE FROM Players");
            migrationBuilder.Sql("DELETE FROM Termins");
            migrationBuilder.Sql("DELETE FROM Notifications");
            migrationBuilder.Sql("DELETE FROM EventSchedules");
            migrationBuilder.Sql("DELETE FROM EventInvitations");
            migrationBuilder.Sql("DELETE FROM EventMembers");
            migrationBuilder.Sql("DELETE FROM Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Termins_TerminId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "TerminAnnouncements");

            migrationBuilder.DropTable(
                name: "TerminApplications");

            migrationBuilder.DropTable(
                name: "Termins");

            migrationBuilder.RenameColumn(
                name: "TerminId",
                table: "Players",
                newName: "MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_TerminId",
                table: "Players",
                newName: "IX_Players_MatchId");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumberOfPlayersExpected = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_EventSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "EventSchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matches_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnouncementType = table.Column<int>(type: "int", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchAnnouncements_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchAnnouncements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_MatchApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchApplications_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchApplications_Users_AppliedByUserId",
                        column: x => x.AppliedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchApplications_Users_CompletedByUserId",
                        column: x => x.CompletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchAnnouncements_MatchId",
                table: "MatchAnnouncements",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchAnnouncements_UserId",
                table: "MatchAnnouncements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchApplications_AppliedByUserId",
                table: "MatchApplications",
                column: "AppliedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchApplications_CompletedByUserId",
                table: "MatchApplications",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchApplications_MatchId",
                table: "MatchApplications",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_EventId",
                table: "Matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_ScheduleId",
                table: "Matches",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Matches_MatchId",
                table: "Players",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Matches_MatchId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "MatchAnnouncements");

            migrationBuilder.DropTable(
                name: "MatchApplications");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "Players",
                newName: "TerminId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_MatchId",
                table: "Players",
                newName: "IX_Players_TerminId");

            migrationBuilder.CreateTable(
                name: "Termins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfPlayersExpected = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SportType = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Termins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Termins_EventSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "EventSchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Termins_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TerminAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnouncementType = table.Column<int>(type: "int", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminAnnouncements_Termins_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TerminAnnouncements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TerminApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Accepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rejected = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TerminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminApplications_Termins_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TerminApplications_Users_AppliedByUserId",
                        column: x => x.AppliedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TerminApplications_Users_CompletedByUserId",
                        column: x => x.CompletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminAnnouncements_TerminId",
                table: "TerminAnnouncements",
                column: "TerminId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminAnnouncements_UserId",
                table: "TerminAnnouncements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_AppliedByUserId",
                table: "TerminApplications",
                column: "AppliedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_CompletedByUserId",
                table: "TerminApplications",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_TerminId",
                table: "TerminApplications",
                column: "TerminId");

            migrationBuilder.CreateIndex(
                name: "IX_Termins_EventId",
                table: "Termins",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Termins_ScheduleId",
                table: "Termins",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Termins_TerminId",
                table: "Players",
                column: "TerminId",
                principalTable: "Termins",
                principalColumn: "Id");
        }
    }
}
