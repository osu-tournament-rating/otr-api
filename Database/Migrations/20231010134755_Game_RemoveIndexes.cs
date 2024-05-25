#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Game_RemoveIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_games_team_type",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_team_type_id",
                table: "games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_games_team_type",
                table: "games",
                column: "team_type");

            migrationBuilder.CreateIndex(
                name: "IX_games_team_type_id",
                table: "games",
                columns: new[] { "team_type", "id" });
        }
    }
}
