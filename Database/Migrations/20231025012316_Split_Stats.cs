#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Split_Stats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "average_opponent_rating",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "average_teammate_rating",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "country_rank_after",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "country_rank_before",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "global_rank_after",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "global_rank_before",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "match_cost",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "percentile_after",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "percentile_before",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "rating_after",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "rating_before",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "rating_change",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "volatility_after",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "volatility_before",
                table: "player_match_statistics");

            migrationBuilder.CreateTable(
                name: "match_rating_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    match_cost = table.Column<double>(type: "double precision", nullable: false),
                    rating_before = table.Column<double>(type: "double precision", nullable: true),
                    rating_after = table.Column<double>(type: "double precision", nullable: false),
                    rating_change = table.Column<double>(type: "double precision", nullable: false),
                    volatility_before = table.Column<double>(type: "double precision", nullable: true),
                    volatility_after = table.Column<double>(type: "double precision", nullable: false),
                    global_rank_before = table.Column<int>(type: "integer", nullable: true),
                    global_rank_after = table.Column<int>(type: "integer", nullable: false),
                    country_rank_before = table.Column<int>(type: "integer", nullable: true),
                    country_rank_after = table.Column<int>(type: "integer", nullable: false),
                    percentile_before = table.Column<double>(type: "double precision", nullable: true),
                    percentile_after = table.Column<double>(type: "double precision", nullable: false),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: false),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_rating_statistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_rating_statistics_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_rating_statistics_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_statistics_match_id",
                table: "match_rating_statistics",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_statistics_player_id",
                table: "match_rating_statistics",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_rating_statistics");

            migrationBuilder.AddColumn<double>(
                name: "average_opponent_rating",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "average_teammate_rating",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "country_rank_after",
                table: "player_match_statistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "country_rank_before",
                table: "player_match_statistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_after",
                table: "player_match_statistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_rank_before",
                table: "player_match_statistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "match_cost",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_after",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "percentile_before",
                table: "player_match_statistics",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "rating_after",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "rating_before",
                table: "player_match_statistics",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "rating_change",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "volatility_after",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "volatility_before",
                table: "player_match_statistics",
                type: "double precision",
                nullable: true);
        }
    }
}
