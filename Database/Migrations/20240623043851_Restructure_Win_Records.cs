using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Win_Records : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "match_type",
                table: "match_win_records");

            migrationBuilder.RenameColumn(
                name: "winner_points",
                table: "match_win_records",
                newName: "winner_score");

            migrationBuilder.RenameColumn(
                name: "loser_points",
                table: "match_win_records",
                newName: "loser_score");

            migrationBuilder.RenameColumn(
                name: "winners",
                table: "game_win_records",
                newName: "winner_roster");

            migrationBuilder.RenameColumn(
                name: "losers",
                table: "game_win_records",
                newName: "loser_roster");

            migrationBuilder.RenameIndex(
                name: "IX_game_win_records_winners",
                table: "game_win_records",
                newName: "IX_game_win_records_winner_roster");

            migrationBuilder.AlterColumn<int>(
                name: "winner_team",
                table: "match_win_records",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "loser_team",
                table: "match_win_records",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "loser_score",
                table: "game_win_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "winner_score",
                table: "game_win_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loser_score",
                table: "game_win_records");

            migrationBuilder.DropColumn(
                name: "winner_score",
                table: "game_win_records");

            migrationBuilder.RenameColumn(
                name: "winner_score",
                table: "match_win_records",
                newName: "winner_points");

            migrationBuilder.RenameColumn(
                name: "loser_score",
                table: "match_win_records",
                newName: "loser_points");

            migrationBuilder.RenameColumn(
                name: "winner_roster",
                table: "game_win_records",
                newName: "winners");

            migrationBuilder.RenameColumn(
                name: "loser_roster",
                table: "game_win_records",
                newName: "losers");

            migrationBuilder.RenameIndex(
                name: "IX_game_win_records_winner_roster",
                table: "game_win_records",
                newName: "IX_game_win_records_winners");

            migrationBuilder.AlterColumn<int>(
                name: "winner_team",
                table: "match_win_records",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "loser_team",
                table: "match_win_records",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "match_type",
                table: "match_win_records",
                type: "integer",
                nullable: true);
        }
    }
}
