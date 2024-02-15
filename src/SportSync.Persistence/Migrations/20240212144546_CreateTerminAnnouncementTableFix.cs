using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateTerminAnnouncementTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminAnnouncement_Termins_TerminId",
                table: "TerminAnnouncement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TerminAnnouncement",
                table: "TerminAnnouncement");

            migrationBuilder.RenameTable(
                name: "TerminAnnouncement",
                newName: "TerminAnnouncements");

            migrationBuilder.RenameIndex(
                name: "IX_TerminAnnouncement_TerminId",
                table: "TerminAnnouncements",
                newName: "IX_TerminAnnouncements_TerminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TerminAnnouncements",
                table: "TerminAnnouncements",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminAnnouncements_UserId",
                table: "TerminAnnouncements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminAnnouncements_Termins_TerminId",
                table: "TerminAnnouncements",
                column: "TerminId",
                principalTable: "Termins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminAnnouncements_Users_UserId",
                table: "TerminAnnouncements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminAnnouncements_Termins_TerminId",
                table: "TerminAnnouncements");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminAnnouncements_Users_UserId",
                table: "TerminAnnouncements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TerminAnnouncements",
                table: "TerminAnnouncements");

            migrationBuilder.DropIndex(
                name: "IX_TerminAnnouncements_UserId",
                table: "TerminAnnouncements");

            migrationBuilder.RenameTable(
                name: "TerminAnnouncements",
                newName: "TerminAnnouncement");

            migrationBuilder.RenameIndex(
                name: "IX_TerminAnnouncements_TerminId",
                table: "TerminAnnouncement",
                newName: "IX_TerminAnnouncement_TerminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TerminAnnouncement",
                table: "TerminAnnouncement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminAnnouncement_Termins_TerminId",
                table: "TerminAnnouncement",
                column: "TerminId",
                principalTable: "Termins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
