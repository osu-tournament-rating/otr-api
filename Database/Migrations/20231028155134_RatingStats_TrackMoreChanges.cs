#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RatingStats_TrackMoreChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "country_rank_change",
                table: "match_rating_statistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_change",
                table: "match_rating_statistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_change",
                table: "match_rating_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "volatility_change",
                table: "match_rating_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country_rank_change",
                table: "match_rating_statistics");

            migrationBuilder.DropColumn(
                name: "global_rank_change",
                table: "match_rating_statistics");

            migrationBuilder.DropColumn(
                name: "percentile_change",
                table: "match_rating_statistics");

            migrationBuilder.DropColumn(
                name: "volatility_change",
                table: "match_rating_statistics");
        }
    }
}
