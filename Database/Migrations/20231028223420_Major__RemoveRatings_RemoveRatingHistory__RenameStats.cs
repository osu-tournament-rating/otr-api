#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Major__RemoveRatings_RemoveRatingHistory__RenameStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_rating_statistics");

            migrationBuilder.DropTable(
                name: "player_match_statistics");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.CreateTable(
                name: "base_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    volatility = table.Column<double>(type: "double precision", nullable: false),
                    percentile = table.Column<double>(type: "double precision", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    country_rank = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("BaseStatistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "BaseStatistics___fkplayerid",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "match_rating_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    match_cost = table.Column<double>(type: "double precision", nullable: false),
                    rating_before = table.Column<double>(type: "double precision", nullable: false),
                    rating_after = table.Column<double>(type: "double precision", nullable: false),
                    rating_change = table.Column<double>(type: "double precision", nullable: false),
                    volatility_before = table.Column<double>(type: "double precision", nullable: false),
                    volatility_after = table.Column<double>(type: "double precision", nullable: false),
                    volatility_change = table.Column<double>(type: "double precision", nullable: false),
                    global_rank_before = table.Column<int>(type: "integer", nullable: false),
                    global_rank_after = table.Column<int>(type: "integer", nullable: false),
                    global_rank_change = table.Column<int>(type: "integer", nullable: false),
                    country_rank_before = table.Column<int>(type: "integer", nullable: false),
                    country_rank_after = table.Column<int>(type: "integer", nullable: false),
                    country_rank_change = table.Column<int>(type: "integer", nullable: false),
                    percentile_before = table.Column<double>(type: "double precision", nullable: false),
                    percentile_after = table.Column<double>(type: "double precision", nullable: false),
                    percentile_change = table.Column<double>(type: "double precision", nullable: false),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: true),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_rating_statistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_rating_stats_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_rating_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_match_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false),
                    average_score = table.Column<int>(type: "integer", nullable: false),
                    average_misses = table.Column<double>(type: "double precision", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    average_placement = table.Column<double>(type: "double precision", nullable: false),
                    games_won = table.Column<int>(type: "integer", nullable: false),
                    games_lost = table.Column<int>(type: "integer", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    opponent_ids = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PlayerMatchStatistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_match_stats_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_match_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_base_stats_player_id",
                table: "base_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ratings_playerid_mode",
                table: "base_stats",
                columns: new[] { "player_id", "mode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_stats_match_id",
                table: "match_rating_stats",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_stats_player_id",
                table: "match_rating_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_stats_match_id",
                table: "player_match_stats",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_stats_player_id",
                table: "player_match_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_stats_player_id_match_id",
                table: "player_match_stats",
                columns: new[] { "player_id", "match_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "base_stats");

            migrationBuilder.DropTable(
                name: "match_rating_stats");

            migrationBuilder.DropTable(
                name: "player_match_stats");

            migrationBuilder.CreateTable(
                name: "match_rating_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: true),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: true),
                    country_rank_after = table.Column<int>(type: "integer", nullable: false),
                    country_rank_before = table.Column<int>(type: "integer", nullable: false),
                    country_rank_change = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "player_match_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    average_misses = table.Column<double>(type: "double precision", nullable: false),
                    average_placement = table.Column<double>(type: "double precision", nullable: false),
                    average_score = table.Column<int>(type: "integer", nullable: false),
                    games_lost = table.Column<int>(type: "integer", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    games_won = table.Column<int>(type: "integer", nullable: false),
                    opponent_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PlayerMatchStatistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_match_statistics_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_match_statistics_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    mu = table.Column<double>(type: "double precision", nullable: false),
                    mu_initial = table.Column<double>(type: "double precision", nullable: false),
                    sigma = table.Column<double>(type: "double precision", nullable: false),
                    sigma_initial = table.Column<double>(type: "double precision", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Ratings_pk", x => x.id);
                    table.ForeignKey(
                        name: "Ratings___fkplayerid",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_statistics_match_id",
                table: "match_rating_statistics",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_statistics_player_id",
                table: "match_rating_statistics",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_statistics_match_id",
                table: "player_match_statistics",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_statistics_player_id",
                table: "player_match_statistics",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_statistics_player_id_match_id",
                table: "player_match_statistics",
                columns: new[] { "player_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_player_id",
                table: "ratings",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ratings_playerid_mode",
                table: "ratings",
                columns: new[] { "player_id", "mode" },
                unique: true);
        }
    }
}
