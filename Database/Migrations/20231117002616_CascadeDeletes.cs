#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats");

            migrationBuilder.DropForeignKey(
                name: "games_matches_id_fk",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores");

            migrationBuilder.AddForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "games_matches_id_fk",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats");

            migrationBuilder.DropForeignKey(
                name: "games_matches_id_fk",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores");

            migrationBuilder.AddForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "games_matches_id_fk",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");
        }
    }
}
