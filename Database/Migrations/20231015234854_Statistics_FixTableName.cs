#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Statistics_FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_statistics_players_player_id",
                table: "player_statistics");

            migrationBuilder.RenameTable(
                name: "player_statistics",
                newName: "player_game_statistics");

            migrationBuilder.RenameIndex(
                name: "IX_player_statistics_player_id_game_id",
                table: "player_game_statistics",
                newName: "IX_player_game_statistics_player_id_game_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_statistics_player_id",
                table: "player_game_statistics",
                newName: "IX_player_game_statistics_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_statistics_game_id",
                table: "player_game_statistics",
                newName: "IX_player_game_statistics_game_id");

            migrationBuilder.AddForeignKey(
                name: "FK_player_game_statistics_players_player_id",
                table: "player_game_statistics",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_game_statistics_players_player_id",
                table: "player_game_statistics");

            migrationBuilder.RenameTable(
                name: "player_game_statistics",
                newName: "player_statistics");

            migrationBuilder.RenameIndex(
                name: "IX_player_game_statistics_player_id_game_id",
                table: "player_statistics",
                newName: "IX_player_statistics_player_id_game_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_game_statistics_player_id",
                table: "player_statistics",
                newName: "IX_player_statistics_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_game_statistics_game_id",
                table: "player_statistics",
                newName: "IX_player_statistics_game_id");

            migrationBuilder.AddForeignKey(
                name: "FK_player_statistics_players_player_id",
                table: "player_statistics",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
