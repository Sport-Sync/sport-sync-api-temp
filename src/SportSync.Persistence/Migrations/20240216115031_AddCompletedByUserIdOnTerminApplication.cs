using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedByUserIdOnTerminApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminAnnouncements_Termins_TerminId",
                table: "TerminAnnouncements");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminApplications_Users_UserId",
                table: "TerminApplications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TerminApplications",
                newName: "CompletedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TerminApplications_UserId",
                table: "TerminApplications",
                newName: "IX_TerminApplications_CompletedByUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "AppliedByUserId",
                table: "TerminApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TerminApplications_AppliedByUserId",
                table: "TerminApplications",
                column: "AppliedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminAnnouncements_Termins_TerminId",
                table: "TerminAnnouncements",
                column: "TerminId",
                principalTable: "Termins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TerminApplications_Users_AppliedByUserId",
                table: "TerminApplications",
                column: "AppliedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminApplications_Users_CompletedByUserId",
                table: "TerminApplications",
                column: "CompletedByUserId",
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
                name: "FK_TerminApplications_Users_AppliedByUserId",
                table: "TerminApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminApplications_Users_CompletedByUserId",
                table: "TerminApplications");

            migrationBuilder.DropIndex(
                name: "IX_TerminApplications_AppliedByUserId",
                table: "TerminApplications");

            migrationBuilder.DropColumn(
                name: "AppliedByUserId",
                table: "TerminApplications");

            migrationBuilder.RenameColumn(
                name: "CompletedByUserId",
                table: "TerminApplications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TerminApplications_CompletedByUserId",
                table: "TerminApplications",
                newName: "IX_TerminApplications_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminAnnouncements_Termins_TerminId",
                table: "TerminAnnouncements",
                column: "TerminId",
                principalTable: "Termins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminApplications_Users_UserId",
                table: "TerminApplications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
