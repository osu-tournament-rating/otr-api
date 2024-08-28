#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "beatmaps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artist = table.Column<string>(type: "text", nullable: false),
                    beatmap_id = table.Column<long>(type: "bigint", nullable: false),
                    bpm = table.Column<double>(type: "double precision", nullable: false),
                    mapper_id = table.Column<long>(type: "bigint", nullable: false),
                    mapper_name = table.Column<string>(type: "text", nullable: false),
                    sr = table.Column<double>(type: "double precision", nullable: false),
                    aim_diff = table.Column<double>(type: "double precision", nullable: true),
                    speed_diff = table.Column<double>(type: "double precision", nullable: true),
                    cs = table.Column<double>(type: "double precision", nullable: false),
                    ar = table.Column<double>(type: "double precision", nullable: false),
                    hp = table.Column<double>(type: "double precision", nullable: false),
                    od = table.Column<double>(type: "double precision", nullable: false),
                    drain_time = table.Column<double>(type: "double precision", nullable: false),
                    length = table.Column<double>(type: "double precision", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    diff_name = table.Column<string>(type: "text", nullable: true),
                    game_mode = table.Column<int>(type: "integer", nullable: false),
                    circle_count = table.Column<int>(type: "integer", nullable: false),
                    slider_count = table.Column<int>(type: "integer", nullable: false),
                    spinner_count = table.Column<int>(type: "integer", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("beatmaps_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verification_info = table.Column<string>(type: "text", nullable: true),
                    verification_source = table.Column<int>(type: "integer", nullable: true),
                    verification_status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("matches_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    rank_standard = table.Column<int>(type: "integer", nullable: true),
                    rank_taiko = table.Column<int>(type: "integer", nullable: true),
                    rank_catch = table.Column<int>(type: "integer", nullable: true),
                    rank_mania = table.Column<int>(type: "integer", nullable: true),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Player_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    beatmap_id = table.Column<int>(type: "integer", nullable: true),
                    play_mode = table.Column<int>(type: "integer", nullable: false),
                    match_type = table.Column<int>(type: "integer", nullable: false),
                    scoring_type = table.Column<int>(type: "integer", nullable: false),
                    team_type = table.Column<int>(type: "integer", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<long>(type: "bigint", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("osugames_pk", x => x.id);
                    table.ForeignKey(
                        name: "games_beatmaps_id_fk",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "games_matches_id_fk",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ratinghistories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    mu = table.Column<double>(type: "double precision", nullable: false),
                    sigma = table.Column<double>(type: "double precision", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RatingHistories_pk", x => x.id);
                    table.ForeignKey(
                        name: "RatingHistories___fkplayerid",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ratinghistories_matches_id_fk",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    mu = table.Column<double>(type: "double precision", nullable: false),
                    sigma = table.Column<double>(type: "double precision", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    mode = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    roles = table.Column<string>(type: "text", nullable: true, comment: "Comma-delimited list of roles (e.g. user, admin, etc.)"),
                    session_token = table.Column<string>(type: "text", nullable: true),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    session_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_pk", x => x.id);
                    table.ForeignKey(
                        name: "Users___fkplayerid",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "match_scores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<long>(type: "bigint", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    count_50 = table.Column<int>(type: "integer", nullable: false),
                    count_100 = table.Column<int>(type: "integer", nullable: false),
                    count_300 = table.Column<int>(type: "integer", nullable: false),
                    count_miss = table.Column<int>(type: "integer", nullable: false),
                    perfect = table.Column<bool>(type: "boolean", nullable: false),
                    pass = table.Column<bool>(type: "boolean", nullable: false),
                    enabled_mods = table.Column<int>(type: "integer", nullable: true),
                    count_katu = table.Column<int>(type: "integer", nullable: false),
                    count_geki = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_scores_pk", x => x.id);
                    table.ForeignKey(
                        name: "match_scores_games_id_fk",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "match_scores_players_id_fk",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "beatmaps_beatmapid",
                table: "beatmaps",
                column: "beatmap_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_beatmap_id",
                table: "games",
                column: "beatmap_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_match_id",
                table: "games",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "osugames_gameid",
                table: "games",
                column: "game_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_player_id",
                table: "match_scores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "match_scores_gameid_playerid",
                table: "match_scores",
                columns: new[] { "game_id", "player_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "osumatches_matchid",
                table: "matches",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Players_osuid",
                table: "players",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_match_id",
                table: "ratinghistories",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ratinghistories_pk",
                table: "ratinghistories",
                columns: new[] { "player_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ratings_playerid_mode",
                table: "ratings",
                columns: new[] { "player_id", "mode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_player_id",
                table: "users",
                column: "player_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "match_scores");

            migrationBuilder.DropTable(
                name: "ratinghistories");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "beatmaps");

            migrationBuilder.DropTable(
                name: "matches");
        }
    }
}
