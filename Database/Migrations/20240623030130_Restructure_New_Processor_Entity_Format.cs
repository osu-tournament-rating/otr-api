using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_New_Processor_Entity_Format : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                TRUNCATE TABLE rating_adjustments RESTART IDENTITY
                """);

            migrationBuilder.DropTable(
                name: "base_stats");

            migrationBuilder.DropTable(
                name: "match_rating_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_rating_adjustments_player_id_ruleset",
                table: "rating_adjustments");

            migrationBuilder.RenameColumn(
                name: "volatility_adjustment_amount",
                table: "rating_adjustments",
                newName: "volatility_delta");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "rating_adjustments",
                newName: "player_rating_id");

            migrationBuilder.RenameColumn(
                name: "rating_adjustment_type",
                table: "rating_adjustments",
                newName: "global_rank_delta");

            migrationBuilder.RenameColumn(
                name: "rating_adjustment_amount",
                table: "rating_adjustments",
                newName: "rating_delta");

            migrationBuilder.AddColumn<int>(
                name: "AdjustmentType",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                name: "match_id",
                table: "rating_adjustments",
                type: "integer",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "player_ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    volatility = table.Column<double>(type: "double precision", nullable: false),
                    percentile = table.Column<double>(type: "double precision", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    country_rank = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_ratings", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_ratings_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rating_adjustments_match_id",
                table: "rating_adjustments",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_rating_adjustments_player_id_match_id",
                table: "rating_adjustments",
                columns: new[] { "player_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rating_adjustments_player_id_timestamp",
                table: "rating_adjustments",
                columns: new[] { "player_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_rating_adjustments_player_rating_id",
                table: "rating_adjustments",
                column: "player_rating_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_ratings_player_id",
                table: "player_ratings",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_ratings_player_id_ruleset",
                table: "player_ratings",
                columns: new[] { "player_id", "ruleset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_ratings_rating",
                table: "player_ratings",
                column: "rating",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_player_ratings_ruleset",
                table: "player_ratings",
                column: "ruleset");

            migrationBuilder.AddForeignKey(
                name: "FK_rating_adjustments_matches_match_id",
                table: "rating_adjustments",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments",
                column: "player_rating_id",
                principalTable: "player_ratings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rating_adjustments_matches_match_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments");

            migrationBuilder.DropTable(
                name: "player_ratings");

            migrationBuilder.DropIndex(
                name: "IX_rating_adjustments_match_id",
                table: "rating_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_rating_adjustments_player_id_match_id",
                table: "rating_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_rating_adjustments_player_id_timestamp",
                table: "rating_adjustments");

            migrationBuilder.DropIndex(
                name: "IX_rating_adjustments_player_rating_id",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "AdjustmentType",
                table: "rating_adjustments");

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
                name: "match_id",
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

            migrationBuilder.RenameColumn(
                name: "volatility_delta",
                table: "rating_adjustments",
                newName: "volatility_adjustment_amount");

            migrationBuilder.RenameColumn(
                name: "rating_delta",
                table: "rating_adjustments",
                newName: "rating_adjustment_amount");

            migrationBuilder.RenameColumn(
                name: "player_rating_id",
                table: "rating_adjustments",
                newName: "ruleset");

            migrationBuilder.RenameColumn(
                name: "global_rank_delta",
                table: "rating_adjustments",
                newName: "rating_adjustment_type");

            migrationBuilder.CreateTable(
                name: "base_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    country_rank = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    match_cost_average = table.Column<double>(type: "double precision", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    percentile = table.Column<double>(type: "double precision", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    volatility = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_base_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_base_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_rating_adjustments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: true),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: true),
                    country_rank_after = table.Column<int>(type: "integer", nullable: false),
                    country_rank_before = table.Column<int>(type: "integer", nullable: false),
                    country_rank_change = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    global_rank_after = table.Column<int>(type: "integer", nullable: false),
                    global_rank_before = table.Column<int>(type: "integer", nullable: false),
                    global_rank_change = table.Column<int>(type: "integer", nullable: false),
                    match_cost = table.Column<double>(type: "double precision", nullable: false),
                    percentile_after = table.Column<double>(type: "double precision", nullable: false),
                    percentile_before = table.Column<double>(type: "double precision", nullable: false),
                    percentile_change = table.Column<double>(type: "double precision", nullable: false),
                    rating_after = table.Column<double>(type: "double precision", nullable: false),
                    rating_before = table.Column<double>(type: "double precision", nullable: false),
                    rating_change = table.Column<double>(type: "double precision", nullable: false),
                    volatility_after = table.Column<double>(type: "double precision", nullable: false),
                    volatility_before = table.Column<double>(type: "double precision", nullable: false),
                    volatility_change = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_rating_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_rating_adjustments_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_rating_adjustments_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rating_adjustments_player_id_ruleset",
                table: "rating_adjustments",
                columns: new[] { "player_id", "ruleset" });

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_player_id",
                table: "base_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_player_id_ruleset",
                table: "base_stats",
                columns: new[] { "player_id", "ruleset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_rating",
                table: "base_stats",
                column: "rating",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_ruleset",
                table: "base_stats",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_adjustments_match_id",
                table: "match_rating_adjustments",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_adjustments_player_id_match_id",
                table: "match_rating_adjustments",
                columns: new[] { "player_id", "match_id" },
                unique: true);
        }
    }
}
