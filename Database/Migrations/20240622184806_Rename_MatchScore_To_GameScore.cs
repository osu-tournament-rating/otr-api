using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Rename_MatchScore_To_GameScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_scores");

            migrationBuilder.CreateTable(
                name: "game_scores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    score = table.Column<long>(type: "bigint", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    count_50 = table.Column<int>(type: "integer", nullable: false),
                    count_100 = table.Column<int>(type: "integer", nullable: false),
                    count_300 = table.Column<int>(type: "integer", nullable: false),
                    count_miss = table.Column<int>(type: "integer", nullable: false),
                    count_katu = table.Column<int>(type: "integer", nullable: false),
                    count_geki = table.Column<int>(type: "integer", nullable: false),
                    pass = table.Column<bool>(type: "boolean", nullable: false),
                    perfect = table.Column<bool>(type: "boolean", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    verification_status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_scores", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_scores_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_scores_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_scores_game_id",
                table: "game_scores",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_scores_player_id",
                table: "game_scores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_scores_player_id_game_id",
                table: "game_scores",
                columns: new[] { "player_id", "game_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_scores");

            migrationBuilder.CreateTable(
                name: "match_scores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    count_100 = table.Column<int>(type: "integer", nullable: false),
                    count_300 = table.Column<int>(type: "integer", nullable: false),
                    count_50 = table.Column<int>(type: "integer", nullable: false),
                    count_geki = table.Column<int>(type: "integer", nullable: false),
                    count_katu = table.Column<int>(type: "integer", nullable: false),
                    count_miss = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    enabled_mods = table.Column<int>(type: "integer", nullable: true),
                    is_valid = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    pass = table.Column<bool>(type: "boolean", nullable: false),
                    perfect = table.Column<bool>(type: "boolean", nullable: false),
                    score = table.Column<long>(type: "bigint", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_scores", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_scores_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_scores_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_game_id",
                table: "match_scores",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_player_id",
                table: "match_scores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_player_id_game_id",
                table: "match_scores",
                columns: new[] { "player_id", "game_id" },
                unique: true);
        }
    }
}
