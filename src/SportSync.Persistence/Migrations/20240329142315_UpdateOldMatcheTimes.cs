using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSync.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOldMatcheTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             UPDATE [Matches]
                SET StartTime = CONVERT(datetimeoffset, CONVERT(varchar(10), [Date], 120) + ' ' + CONVERT(varchar(12), [StartTime], 114)),
	            EndTime = CONVERT(datetimeoffset, CONVERT(varchar(10), [Date], 120) + ' ' + CONVERT(varchar(12), [EndTime], 114))
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
