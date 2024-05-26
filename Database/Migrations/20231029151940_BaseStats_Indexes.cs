#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class BaseStats_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_base_stats_mode",
                table: "base_stats",
                column: "mode");

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_rating",
                table: "base_stats",
                column: "rating",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_base_stats_mode",
                table: "base_stats");

            migrationBuilder.DropIndex(
                name: "IX_base_stats_rating",
                table: "base_stats");
        }
    }
}
