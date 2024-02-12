using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateTerminApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TerminApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_TerminApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminApplications_Termins_TerminId",
                        column: x => x.TerminId,
                        principalTable: "Termins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TerminApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_TerminId",
                table: "TerminApplications",
                column: "TerminId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_UserId",
                table: "TerminApplications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminApplications");
        }
    }
}
