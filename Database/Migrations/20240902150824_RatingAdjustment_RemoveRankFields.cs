using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RatingAdjustment_RemoveRankFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country_rank_after",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "country_rank_before",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "country_rank_delta",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "global_rank_after",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "global_rank_before",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "global_rank_delta",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "percentile_after",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "percentile_before",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "percentile_delta",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "rating_delta",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "volatility_delta",
                table: "rating_adjustments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "country_rank_after",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "country_rank_before",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "country_rank_delta",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_after",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_before",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_delta",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_after",
                table: "rating_adjustments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_before",
                table: "rating_adjustments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_delta",
                table: "rating_adjustments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "rating_delta",
                table: "rating_adjustments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "volatility_delta",
                table: "rating_adjustments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
