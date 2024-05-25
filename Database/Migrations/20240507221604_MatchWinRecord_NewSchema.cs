#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchWinRecord_NewSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "team_red",
                table: "match_win_records",
                newName: "winner_roster");

            migrationBuilder.RenameColumn(
                name: "team_blue",
                table: "match_win_records",
                newName: "loser_roster");

            migrationBuilder.RenameColumn(
                name: "red_points",
                table: "match_win_records",
                newName: "winner_points");

            migrationBuilder.RenameColumn(
                name: "blue_points",
                table: "match_win_records",
                newName: "loser_points");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team_red",
                table: "match_win_records",
                newName: "IX_match_win_records_winner_roster");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_team_blue",
                table: "match_win_records",
                newName: "IX_match_win_records_loser_roster");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "winner_roster",
                table: "match_win_records",
                newName: "team_red");

            migrationBuilder.RenameColumn(
                name: "winner_points",
                table: "match_win_records",
                newName: "red_points");

            migrationBuilder.RenameColumn(
                name: "loser_roster",
                table: "match_win_records",
                newName: "team_blue");

            migrationBuilder.RenameColumn(
                name: "loser_points",
                table: "match_win_records",
                newName: "blue_points");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_winner_roster",
                table: "match_win_records",
                newName: "IX_match_win_records_team_red");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_loser_roster",
                table: "match_win_records",
                newName: "IX_match_win_records_team_blue");
        }
    }
}
