using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameAnnouncementColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements DROP CONSTRAINT CHK_NumberOfPlayersAccepted");

            migrationBuilder.RenameColumn(
                name: "NumberOfPlayersLimit",
                table: "MatchAnnouncements",
                newName: "PlayerLimit");

            migrationBuilder.RenameColumn(
                name: "NumberOfPlayersAccepted",
                table: "MatchAnnouncements",
                newName: "AcceptedPlayersCount");

            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements ADD CONSTRAINT CHK_AcceptedPlayersCount CHECK (AcceptedPlayersCount <= PlayerLimit)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements DROP CONSTRAINT CHK_AcceptedPlayersCount");
            
            migrationBuilder.RenameColumn(
                name: "PlayerLimit",
                table: "MatchAnnouncements",
                newName: "NumberOfPlayersLimit");

            migrationBuilder.RenameColumn(
                name: "AcceptedPlayersCount",
                table: "MatchAnnouncements",
                newName: "NumberOfPlayersAccepted");
         
            migrationBuilder.Sql("ALTER TABLE MatchAnnouncements ADD CONSTRAINT CHK_NumberOfPlayersAccepted CHECK (NumberOfPlayersAccepted <= NumberOfPlayersLimit)");
        }
    }
}
