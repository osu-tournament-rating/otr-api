#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class BaseStats_Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ratings_playerid_mode",
                table: "base_stats",
                newName: "IX_base_stats_player_id_mode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_base_stats_player_id_mode",
                table: "base_stats",
                newName: "ratings_playerid_mode");
        }
    }
}
