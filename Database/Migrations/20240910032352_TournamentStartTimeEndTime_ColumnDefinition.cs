using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TournamentStartTimeEndTime_ColumnDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "tournaments",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "tournaments",
                newName: "end_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "tournaments",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "tournaments",
                newName: "EndTime");
        }
    }
}
