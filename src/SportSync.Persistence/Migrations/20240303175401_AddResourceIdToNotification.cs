using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Actions",
                table: "Notifications",
                newName: "Commands");

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Commands",
                table: "Notifications",
                newName: "Actions");
        }
    }
}
