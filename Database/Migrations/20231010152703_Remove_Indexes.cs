#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratings_mode",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_ratings_mu",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_ratings_player_id_mode",
                table: "ratings");

            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_created",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_mode",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_mu",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_players_country",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_username",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_matches_start_time",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_matches_verification_status",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_enabled_mods",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_game_id_team",
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

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_player_id",
                table: "ratinghistories",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_player_id",
                table: "ratinghistories");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_mode",
                table: "ratings",
                column: "mode");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_mu",
                table: "ratings",
                column: "mu");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_player_id_mode",
                table: "ratings",
                columns: new[] { "player_id", "mode" });

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_created",
                table: "ratinghistories",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_mode",
                table: "ratinghistories",
                column: "mode");

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_mu",
                table: "ratinghistories",
                column: "mu");

            migrationBuilder.CreateIndex(
                name: "IX_players_country",
                table: "players",
                column: "country");

            migrationBuilder.CreateIndex(
                name: "IX_players_username",
                table: "players",
                column: "username");

            migrationBuilder.CreateIndex(
                name: "IX_matches_start_time",
                table: "matches",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_matches_verification_status",
                table: "matches",
                column: "verification_status");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_enabled_mods",
                table: "match_scores",
                column: "enabled_mods");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_game_id_team",
                table: "match_scores",
                columns: new[] { "game_id", "team" });

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
    }
}
