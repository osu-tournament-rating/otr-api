﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        private static readonly string[] columns = new[] { "player_id", "game_id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "beatmaps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    has_data = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    mapper_id = table.Column<long>(type: "bigint", nullable: false),
                    mapper_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    artist = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    diff_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ranked_status = table.Column<int>(type: "integer", nullable: false),
                    sr = table.Column<double>(type: "double precision", nullable: false),
                    bpm = table.Column<double>(type: "double precision", nullable: false),
                    cs = table.Column<double>(type: "double precision", nullable: false),
                    ar = table.Column<double>(type: "double precision", nullable: false),
                    hp = table.Column<double>(type: "double precision", nullable: false),
                    od = table.Column<double>(type: "double precision", nullable: false),
                    length = table.Column<double>(type: "double precision", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    circle_count = table.Column<int>(type: "integer", nullable: false),
                    slider_count = table.Column<int>(type: "integer", nullable: false),
                    spinner_count = table.Column<int>(type: "integer", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beatmaps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    country = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    default_ruleset = table.Column<int>(type: "integer", nullable: false),
                    osu_last_fetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    osu_track_last_fetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ruleset_data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scopes = table.Column<string[]>(type: "text[]", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rate_limit_overrides = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "oauth_clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    scopes = table.Column<string[]>(type: "text[]", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rate_limit_overrides = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_clients", x => x.id);
                    table.ForeignKey(
                        name: "FK_oauth_clients_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    abbreviation = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    forum_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    rank_range_lower_bound = table.Column<int>(type: "integer", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    lobby_size = table.Column<int>(type: "integer", nullable: false),
                    verification_status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    processing_status = table.Column<int>(type: "integer", nullable: false),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    submitted_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments", x => x.id);
                    table.ForeignKey(
                        name: "FK_tournaments_users_submitted_by_user_id",
                        column: x => x.submitted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tournaments_users_verified_by_user_id",
                        column: x => x.verified_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    default_ruleset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    default_ruleset_controlled = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_settings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    verification_status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    processing_status = table.Column<int>(type: "integer", nullable: false),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    submitted_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_tournaments_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matches_users_submitted_by_user_id",
                        column: x => x.submitted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_matches_users_verified_by_user_id",
                        column: x => x.verified_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "player_tournament_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    average_rating_delta = table.Column<double>(type: "double precision", nullable: false),
                    average_match_cost = table.Column<double>(type: "double precision", nullable: false),
                    average_score = table.Column<int>(type: "integer", nullable: false),
                    average_placement = table.Column<double>(type: "double precision", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    matches_played = table.Column<int>(type: "integer", nullable: false),
                    matches_won = table.Column<int>(type: "integer", nullable: false),
                    matches_lost = table.Column<int>(type: "integer", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    games_won = table.Column<int>(type: "integer", nullable: false),
                    games_lost = table.Column<int>(type: "integer", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_tournament_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_tournament_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_tournament_stats_tournaments_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ref_id_lock = table.Column<int>(type: "integer", nullable: false),
                    ref_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournament_audits", x => x.id);
                    table.ForeignKey(
                        name: "FK_tournament_audits_tournaments_ref_id",
                        column: x => x.ref_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    scoring_type = table.Column<int>(type: "integer", nullable: false),
                    team_type = table.Column<int>(type: "integer", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    post_mod_sr = table.Column<double>(type: "double precision", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    verification_status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    processing_status = table.Column<int>(type: "integer", nullable: false),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    beatmap_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_games_beatmaps_beatmap_id",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_games_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ref_id_lock = table.Column<int>(type: "integer", nullable: false),
                    ref_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_audits", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_audits_matches_ref_id",
                        column: x => x.ref_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "match_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    winner_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    loser_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: false),
                    loser_team = table.Column<int>(type: "integer", nullable: false),
                    winner_score = table.Column<int>(type: "integer", nullable: false),
                    loser_score = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_win_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_win_records_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "player_match_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    match_cost = table.Column<double>(type: "double precision", nullable: false),
                    average_score = table.Column<double>(type: "double precision", nullable: false),
                    average_placement = table.Column<double>(type: "double precision", nullable: false),
                    average_misses = table.Column<double>(type: "double precision", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    games_won = table.Column<int>(type: "integer", nullable: false),
                    games_lost = table.Column<int>(type: "integer", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    opponent_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_match_stats", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "rating_adjustments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    adjustment_type = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rating_before = table.Column<double>(type: "double precision", nullable: false),
                    rating_after = table.Column<double>(type: "double precision", nullable: false),
                    volatility_before = table.Column<double>(type: "double precision", nullable: false),
                    volatility_after = table.Column<double>(type: "double precision", nullable: false),
                    player_rating_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    match_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rating_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "FK_rating_adjustments_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rating_adjustments_player_ratings_player_rating_id",
                        column: x => x.player_rating_id,
                        principalTable: "player_ratings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rating_adjustments_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ref_id_lock = table.Column<int>(type: "integer", nullable: false),
                    ref_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_audits", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_audits_games_ref_id",
                        column: x => x.ref_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "game_scores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    score = table.Column<int>(type: "integer", nullable: false),
                    placement = table.Column<int>(type: "integer", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: false),
                    count_50 = table.Column<int>(type: "integer", nullable: false),
                    count_100 = table.Column<int>(type: "integer", nullable: false),
                    count_300 = table.Column<int>(type: "integer", nullable: false),
                    count_miss = table.Column<int>(type: "integer", nullable: false),
                    count_katu = table.Column<int>(type: "integer", nullable: false),
                    count_geki = table.Column<int>(type: "integer", nullable: false),
                    pass = table.Column<bool>(type: "boolean", nullable: false),
                    perfect = table.Column<bool>(type: "boolean", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    verification_status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    processing_status = table.Column<int>(type: "integer", nullable: false),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
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

            migrationBuilder.CreateTable(
                name: "game_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    winner_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    loser_roster = table.Column<int[]>(type: "integer[]", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: false),
                    loser_team = table.Column<int>(type: "integer", nullable: false),
                    winner_score = table.Column<int>(type: "integer", nullable: false),
                    loser_score = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_win_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_win_records_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_score_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ref_id_lock = table.Column<int>(type: "integer", nullable: false),
                    ref_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_score_audits", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_score_audits_game_scores_ref_id",
                        column: x => x.ref_id,
                        principalTable: "game_scores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beatmaps_osu_id",
                table: "beatmaps",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_audits_ref_id",
                table: "game_audits",
                column: "ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_score_audits_ref_id",
                table: "game_score_audits",
                column: "ref_id");

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
                columns: columns,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_win_records_game_id",
                table: "game_win_records",
                column: "game_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_win_records_winner_roster",
                table: "game_win_records",
                column: "winner_roster");

            migrationBuilder.CreateIndex(
                name: "IX_games_beatmap_id",
                table: "games",
                column: "beatmap_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_match_id",
                table: "games",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_games_osu_id",
                table: "games",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_start_time",
                table: "games",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_match_audits_ref_id",
                table: "match_audits",
                column: "ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_loser_roster",
                table: "match_win_records",
                column: "loser_roster");

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_match_id",
                table: "match_win_records",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_winner_roster",
                table: "match_win_records",
                column: "winner_roster");

            migrationBuilder.CreateIndex(
                name: "IX_matches_osu_id",
                table: "matches",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matches_submitted_by_user_id",
                table: "matches",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_tournament_id",
                table: "matches",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_verified_by_user_id",
                table: "matches",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_clients_user_id",
                table: "oauth_clients",
                column: "user_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_player_match_stats_player_id_won",
                table: "player_match_stats",
                columns: new[] { "player_id", "won" });

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
                descending: Array.Empty<bool>());

            migrationBuilder.CreateIndex(
                name: "IX_player_ratings_ruleset",
                table: "player_ratings",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_player_tournament_stats_player_id_tournament_id",
                table: "player_tournament_stats",
                columns: new[] { "player_id", "tournament_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_tournament_stats_tournament_id",
                table: "player_tournament_stats",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_osu_id",
                table: "players",
                column: "osu_id",
                unique: true);

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
                name: "IX_tournament_audits_ref_id",
                table: "tournament_audits",
                column: "ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_name_abbreviation",
                table: "tournaments",
                columns: new[] { "name", "abbreviation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_ruleset",
                table: "tournaments",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_submitted_by_user_id",
                table: "tournaments",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_user_id",
                table: "user_settings",
                column: "user_id",
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
                name: "game_audits");

            migrationBuilder.DropTable(
                name: "game_score_audits");

            migrationBuilder.DropTable(
                name: "game_win_records");

            migrationBuilder.DropTable(
                name: "match_audits");

            migrationBuilder.DropTable(
                name: "match_win_records");

            migrationBuilder.DropTable(
                name: "oauth_clients");

            migrationBuilder.DropTable(
                name: "player_match_stats");

            migrationBuilder.DropTable(
                name: "player_tournament_stats");

            migrationBuilder.DropTable(
                name: "rating_adjustments");

            migrationBuilder.DropTable(
                name: "tournament_audits");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "game_scores");

            migrationBuilder.DropTable(
                name: "player_ratings");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "beatmaps");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
