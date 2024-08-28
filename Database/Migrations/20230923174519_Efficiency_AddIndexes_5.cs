#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ratings_player_id",
                table: "ratings",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_game_id",
                table: "games",
                column: "game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratings_player_id",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_games_game_id",
                table: "games");
        }
    }
}
