#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ratings_mode",
                table: "ratings",
                column: "mode");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_game_id",
                table: "match_scores",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_team_type",
                table: "games",
                column: "team_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratings_mode",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_game_id",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_games_team_type",
                table: "games");
        }
    }
}
