using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_mode",
                table: "ratinghistories",
                column: "mode");

            migrationBuilder.CreateIndex(
                name: "IX_players_osu_id",
                table: "players",
                column: "osu_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_verification_status",
                table: "matches",
                column: "verification_status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratinghistories_mode",
                table: "ratinghistories");

            migrationBuilder.DropIndex(
                name: "IX_players_osu_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_matches_verification_status",
                table: "matches");
        }
    }
}
