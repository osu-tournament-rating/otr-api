using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxFiltersToFilterReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "max_matches_played",
                table: "filter_reports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "max_tournaments_played",
                table: "filter_reports",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_matches_played",
                table: "filter_reports");

            migrationBuilder.DropColumn(
                name: "max_tournaments_played",
                table: "filter_reports");
        }
    }
}
