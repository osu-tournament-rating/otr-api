using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchAndGameWinRecordRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games");

            migrationBuilder.DropTable(
                name: "game_win_records");

            migrationBuilder.DropTable(
                name: "match_win_records");

            migrationBuilder.CreateTable(
                name: "game_rosters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_rosters", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_rosters_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_rosters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_rosters", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_rosters_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_game_rosters_game_id",
                table: "game_rosters",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_rosters_game_id_roster",
                table: "game_rosters",
                columns: ["game_id", "roster"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_rosters_roster",
                table: "game_rosters",
                column: "roster");

            migrationBuilder.CreateIndex(
                name: "ix_match_rosters_match_id",
                table: "match_rosters",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_rosters_match_id_roster",
                table: "match_rosters",
                columns: ["match_id", "roster"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_rosters_roster",
                table: "match_rosters",
                column: "roster");

            migrationBuilder.AddForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games");

            migrationBuilder.DropTable(
                name: "game_rosters");

            migrationBuilder.DropTable(
                name: "match_rosters");

            migrationBuilder.CreateTable(
                name: "game_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    loser_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    loser_score = table.Column<int>(type: "integer", nullable: false),
                    loser_team = table.Column<int>(type: "integer", nullable: false),
                    winner_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    winner_score = table.Column<int>(type: "integer", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_win_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_win_records_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    loser_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    loser_score = table.Column<int>(type: "integer", nullable: false),
                    loser_team = table.Column<int>(type: "integer", nullable: false),
                    winner_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    winner_score = table.Column<int>(type: "integer", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_win_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_win_records_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_game_win_records_game_id",
                table: "game_win_records",
                column: "game_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_win_records_winner_roster",
                table: "game_win_records",
                column: "winner_roster");

            migrationBuilder.CreateIndex(
                name: "ix_match_win_records_loser_roster",
                table: "match_win_records",
                column: "loser_roster");

            migrationBuilder.CreateIndex(
                name: "ix_match_win_records_match_id",
                table: "match_win_records",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_match_win_records_winner_roster",
                table: "match_win_records",
                column: "winner_roster");

            migrationBuilder.AddForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
