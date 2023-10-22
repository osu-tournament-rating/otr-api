using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Statistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_match_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false),
                    match_cost = table.Column<double>(type: "double precision", nullable: false),
                    points_earned = table.Column<int>(type: "integer", nullable: false),
                    rating_before = table.Column<double>(type: "double precision", nullable: true),
                    rating_after = table.Column<double>(type: "double precision", nullable: false),
                    rating_change = table.Column<double>(type: "double precision", nullable: false),
                    volatility_before = table.Column<double>(type: "double precision", nullable: true),
                    volatility_after = table.Column<double>(type: "double precision", nullable: false),
                    average_score = table.Column<int>(type: "integer", nullable: false),
                    average_misses = table.Column<int>(type: "integer", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    global_rank_before = table.Column<int>(type: "integer", nullable: true),
                    global_rank_after = table.Column<int>(type: "integer", nullable: false),
                    country_rank_before = table.Column<int>(type: "integer", nullable: true),
                    country_rank_after = table.Column<int>(type: "integer", nullable: false),
                    percentile_before = table.Column<double>(type: "double precision", nullable: true),
                    percentile_after = table.Column<double>(type: "double precision", nullable: false)
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
                name: "player_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: false),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: false),
                    placing = table.Column<int>(type: "integer", nullable: false),
                    played_hr = table.Column<bool>(type: "boolean", nullable: false),
                    played_hd = table.Column<bool>(type: "boolean", nullable: false),
                    played_dt = table.Column<bool>(type: "boolean", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    opponent_ids = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PlayerGameStatistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_statistics_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "games_player_statistics_game_id_fk",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_games_start_time",
                table: "games",
                column: "start_time");

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
                name: "IX_player_statistics_game_id",
                table: "player_statistics",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_statistics_player_id",
                table: "player_statistics",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_statistics_player_id_game_id",
                table: "player_statistics",
                columns: new[] { "player_id", "game_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_match_statistics");

            migrationBuilder.DropTable(
                name: "player_statistics");

            migrationBuilder.DropIndex(
                name: "IX_games_start_time",
                table: "games");
        }
    }
}
