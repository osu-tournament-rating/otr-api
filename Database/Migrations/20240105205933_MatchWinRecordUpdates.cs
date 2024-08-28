#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchWinRecordUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "team2_points",
                table: "match_win_records",
                newName: "red_points");

            migrationBuilder.RenameColumn(
                name: "team2",
                table: "match_win_records",
                newName: "team_red");

            migrationBuilder.RenameColumn(
                name: "team1_points",
                table: "match_win_records",
                newName: "blue_points");

            migrationBuilder.RenameColumn(
                name: "team1",
                table: "match_win_records",
                newName: "team_blue");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team2",
                table: "match_win_records",
                newName: "IX_match_win_records_team_red");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team1",
                table: "match_win_records",
                newName: "IX_match_win_records_team_blue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "team_red",
                table: "match_win_records",
                newName: "team2");

            migrationBuilder.RenameColumn(
                name: "team_blue",
                table: "match_win_records",
                newName: "team1");

            migrationBuilder.RenameColumn(
                name: "red_points",
                table: "match_win_records",
                newName: "team2_points");

            migrationBuilder.RenameColumn(
                name: "blue_points",
                table: "match_win_records",
                newName: "team1_points");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team_red",
                table: "match_win_records",
                newName: "IX_match_win_records_team2");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team_blue",
                table: "match_win_records",
                newName: "IX_match_win_records_team1");
        }
    }
}
