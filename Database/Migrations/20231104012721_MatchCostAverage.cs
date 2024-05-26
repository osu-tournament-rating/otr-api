#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchCostAverage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "match_cost_average",
                table: "base_stats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "match_cost_average",
                table: "base_stats");
        }
    }
}
