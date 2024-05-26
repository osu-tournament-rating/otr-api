#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficiency_AddIndexes_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ratings_player_id_mode",
                table: "ratings",
                columns: new[] { "player_id", "mode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratings_player_id_mode",
                table: "ratings");
        }
    }
}
