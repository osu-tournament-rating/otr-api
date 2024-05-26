#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchSCore_match_scores_games_id_fk_SetCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores");

            migrationBuilder.AddForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores");

            migrationBuilder.AddForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id");
        }
    }
}
