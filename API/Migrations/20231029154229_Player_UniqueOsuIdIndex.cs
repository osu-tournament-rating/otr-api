using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Player_UniqueOsuIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_players_osu_id",
                table: "players");

            migrationBuilder.CreateIndex(
                name: "IX_players_osu_id",
                table: "players",
                column: "osu_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_players_osu_id",
                table: "players");

            migrationBuilder.CreateIndex(
                name: "IX_players_osu_id",
                table: "players",
                column: "osu_id");
        }
    }
}
