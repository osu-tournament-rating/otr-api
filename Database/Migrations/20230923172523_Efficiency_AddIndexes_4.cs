#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ratings_mu",
                table: "ratings",
                column: "mu");

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_created",
                table: "ratinghistories",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_mu",
                table: "ratinghistories",
                column: "mu");

            migrationBuilder.CreateIndex(
                name: "IX_matches_start_time",
                table: "matches",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_enabled_mods",
                table: "match_scores",
                column: "enabled_mods");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_team",
                table: "match_scores",
                column: "team");

            migrationBuilder.CreateIndex(
                name: "IX_games_play_mode",
                table: "games",
                column: "play_mode");

            migrationBuilder.CreateIndex(
                name: "IX_games_start_time",
                table: "games",
                column: "start_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratings_mu",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_created",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_mu",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_matches_start_time",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_enabled_mods",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_team",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_games_play_mode",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_start_time",
                table: "games");
        }
    }
}
