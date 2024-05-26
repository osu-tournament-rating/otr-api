#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_match_scores_game_id_team",
                table: "match_scores",
                columns: new[] { "game_id", "team" });

            migrationBuilder.CreateIndex(
                name: "IX_games_team_type_id",
                table: "games",
                columns: new[] { "team_type", "id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_match_scores_game_id_team",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_games_team_type_id",
                table: "games");
        }
    }
}
