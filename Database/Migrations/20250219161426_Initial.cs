﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: ""),
                    country = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false, defaultValue: ""),
                    default_ruleset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    osu_last_fetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    osu_track_last_fetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "beatmapsets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    creator_id = table.Column<int>(type: "integer", nullable: true),
                    artist = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ranked_status = table.Column<int>(type: "integer", nullable: false),
                    ranked_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    submitted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_beatmapsets", x => x.id);
                    table.ForeignKey(
                        name: "fk_beatmapsets_players_creator_id",
                        column: x => x.creator_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_highest_ranks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    global_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    country_rank = table.Column<int>(type: "integer", nullable: false),
                    country_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_highest_ranks", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_highest_ranks_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_osu_ruleset_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    pp = table.Column<double>(type: "double precision", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    earliest_global_rank = table.Column<int>(type: "integer", nullable: true),
                    earliest_global_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_osu_ruleset_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_osu_ruleset_data_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.PrimaryKey("pk_player_ratings", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_ratings_players_player_id",
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
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    scopes = table.Column<string[]>(type: "text[]", nullable: false, defaultValue: new string[0]),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "beatmaps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_id = table.Column<long>(type: "bigint", nullable: false),
                    has_data = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    ranked_status = table.Column<int>(type: "integer", nullable: false),
                    diff_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    total_length = table.Column<long>(type: "bigint", nullable: false),
                    drain_length = table.Column<int>(type: "integer", nullable: false),
                    bpm = table.Column<double>(type: "double precision", nullable: false),
                    count_circle = table.Column<int>(type: "integer", nullable: false),
                    count_slider = table.Column<int>(type: "integer", nullable: false),
                    count_spinner = table.Column<int>(type: "integer", nullable: false),
                    cs = table.Column<double>(type: "double precision", nullable: false),
                    hp = table.Column<double>(type: "double precision", nullable: false),
                    od = table.Column<double>(type: "double precision", nullable: false),
                    ar = table.Column<double>(type: "double precision", nullable: false),
                    sr = table.Column<double>(type: "double precision", nullable: false),
                    max_combo = table.Column<int>(type: "integer", nullable: true),
                    beatmapset_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_beatmaps", x => x.id);
                    table.ForeignKey(
                        name: "fk_beatmaps_beatmapsets_beatmapset_id",
                        column: x => x.beatmapset_id,
                        principalTable: "beatmapsets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "o_auth_clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    scopes = table.Column<string[]>(type: "text[]", nullable: false),
                    rate_limit_override = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_o_auth_clients", x => x.id);
                    table.ForeignKey(
                        name: "fk_o_auth_clients_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_admin_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_admin_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_admin_notes_players_reference_id",
                        column: x => x.reference_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_admin_notes_users_admin_user_id",
                        column: x => x.admin_user_id,
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
                    verification_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    processing_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    submitted_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tournaments", x => x.id);
                    table.ForeignKey(
                        name: "fk_tournaments_users_submitted_by_user_id",
                        column: x => x.submitted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_tournaments_users_verified_by_user_id",
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
                    default_ruleset_is_controlled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_settings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beatmap_attributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    sr = table.Column<double>(type: "double precision", nullable: false),
                    beatmap_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_beatmap_attributes", x => x.id);
                    table.ForeignKey(
                        name: "fk_beatmap_attributes_beatmaps_beatmap_id",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "join_beatmap_creators",
                columns: table => new
                {
                    created_beatmaps_id = table.Column<int>(type: "integer", nullable: false),
                    creators_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_join_beatmap_creators", x => new { x.created_beatmaps_id, x.creators_id });
                    table.ForeignKey(
                        name: "fk_join_beatmap_creators_beatmaps_created_beatmaps_id",
                        column: x => x.created_beatmaps_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_join_beatmap_creators_players_creators_id",
                        column: x => x.creators_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "o_auth_client_admin_note",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_o_auth_client_admin_note", x => x.id);
                    table.ForeignKey(
                        name: "fk_o_auth_client_admin_note_o_auth_clients_reference_id",
                        column: x => x.reference_id,
                        principalTable: "o_auth_clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "join_pooled_beatmaps",
                columns: table => new
                {
                    pooled_beatmaps_id = table.Column<int>(type: "integer", nullable: false),
                    tournaments_pooled_in_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_join_pooled_beatmaps", x => new { x.pooled_beatmaps_id, x.tournaments_pooled_in_id });
                    table.ForeignKey(
                        name: "fk_join_pooled_beatmaps_beatmaps_pooled_beatmaps_id",
                        column: x => x.pooled_beatmaps_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_join_pooled_beatmaps_tournaments_tournaments_pooled_in_id",
                        column: x => x.tournaments_pooled_in_id,
                        principalTable: "tournaments",
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
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    verification_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    warning_flags = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    processing_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    submitted_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matches", x => x.id);
                    table.ForeignKey(
                        name: "fk_matches_tournaments_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_matches_users_submitted_by_user_id",
                        column: x => x.submitted_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_matches_users_verified_by_user_id",
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
                    table.PrimaryKey("pk_player_tournament_stats", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_tournament_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_tournament_stats_tournaments_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament_admin_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tournament_admin_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_tournament_admin_notes_tournaments_reference_id",
                        column: x => x.reference_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tournament_admin_notes_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "users",
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
                    reference_id_lock = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tournament_audits", x => x.id);
                    table.ForeignKey(
                        name: "fk_tournament_audits_tournaments_reference_id",
                        column: x => x.reference_id,
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
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    verification_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    warning_flags = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    processing_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    beatmap_id = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_games_beatmaps_beatmap_id",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_games_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match_admin_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_admin_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_admin_notes_matches_reference_id",
                        column: x => x.reference_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_match_admin_notes_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "users",
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
                    reference_id_lock = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_match_audits", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_audits_matches_reference_id",
                        column: x => x.reference_id,
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
                    table.PrimaryKey("pk_match_win_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_match_win_records_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.PrimaryKey("pk_player_match_stats", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_match_stats_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_match_stats_players_player_id",
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
                    ruleset = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_rating_adjustments", x => x.id);
                    table.ForeignKey(
                        name: "fk_rating_adjustments_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rating_adjustments_player_ratings_player_rating_id",
                        column: x => x.player_rating_id,
                        principalTable: "player_ratings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rating_adjustments_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_admin_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_admin_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_admin_notes_games_reference_id",
                        column: x => x.reference_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_admin_notes_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "users",
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
                    reference_id_lock = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_audits", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_audits_games_reference_id",
                        column: x => x.reference_id,
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
                    count50 = table.Column<int>(type: "integer", nullable: false),
                    count100 = table.Column<int>(type: "integer", nullable: false),
                    count300 = table.Column<int>(type: "integer", nullable: false),
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
                    last_processing_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    rejection_reason = table.Column<int>(type: "integer", nullable: false),
                    processing_status = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_scores", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_scores_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_scores_players_player_id",
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
                    table.PrimaryKey("pk_game_win_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_win_records_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_score_admin_notes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "text", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: false),
                    admin_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_score_admin_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_score_admin_notes_game_scores_reference_id",
                        column: x => x.reference_id,
                        principalTable: "game_scores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_score_admin_notes_users_admin_user_id",
                        column: x => x.admin_user_id,
                        principalTable: "users",
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
                    reference_id_lock = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_score_audits", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_score_audits_game_scores_reference_id",
                        column: x => x.reference_id,
                        principalTable: "game_scores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_beatmap_attributes_beatmap_id_mods",
                table: "beatmap_attributes",
                columns: new[] { "beatmap_id", "mods" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_beatmaps_beatmapset_id",
                table: "beatmaps",
                column: "beatmapset_id");

            migrationBuilder.CreateIndex(
                name: "ix_beatmaps_osu_id",
                table: "beatmaps",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_beatmapsets_creator_id",
                table: "beatmapsets",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_beatmapsets_osu_id",
                table: "beatmapsets",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_admin_notes_admin_user_id",
                table: "game_admin_notes",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_admin_notes_reference_id",
                table: "game_admin_notes",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_audits_reference_id",
                table: "game_audits",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_admin_notes_admin_user_id",
                table: "game_score_admin_notes",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_admin_notes_reference_id",
                table: "game_score_admin_notes",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_audits_reference_id",
                table: "game_score_audits",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_scores_game_id",
                table: "game_scores",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_scores_player_id",
                table: "game_scores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_scores_player_id_game_id",
                table: "game_scores",
                columns: new[] { "player_id", "game_id" },
                unique: true);

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
                name: "ix_games_beatmap_id",
                table: "games",
                column: "beatmap_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_match_id",
                table: "games",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_osu_id",
                table: "games",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_games_start_time",
                table: "games",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "ix_join_beatmap_creators_creators_id",
                table: "join_beatmap_creators",
                column: "creators_id");

            migrationBuilder.CreateIndex(
                name: "ix_join_pooled_beatmaps_tournaments_pooled_in_id",
                table: "join_pooled_beatmaps",
                column: "tournaments_pooled_in_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_admin_notes_admin_user_id",
                table: "match_admin_notes",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_admin_notes_reference_id",
                table: "match_admin_notes",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_audits_reference_id",
                table: "match_audits",
                column: "reference_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_matches_osu_id",
                table: "matches",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_matches_submitted_by_user_id",
                table: "matches",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_matches_tournament_id",
                table: "matches",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "ix_matches_verified_by_user_id",
                table: "matches",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_o_auth_client_admin_note_reference_id",
                table: "o_auth_client_admin_note",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_o_auth_clients_user_id",
                table: "o_auth_clients",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_admin_notes_admin_user_id",
                table: "player_admin_notes",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_admin_notes_reference_id",
                table: "player_admin_notes",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_highest_ranks_country_rank",
                table: "player_highest_ranks",
                column: "country_rank",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_player_highest_ranks_global_rank",
                table: "player_highest_ranks",
                column: "global_rank",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_player_highest_ranks_player_id_ruleset",
                table: "player_highest_ranks",
                columns: new[] { "player_id", "ruleset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_match_stats_match_id",
                table: "player_match_stats",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_match_stats_player_id",
                table: "player_match_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_match_stats_player_id_match_id",
                table: "player_match_stats",
                columns: new[] { "player_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_match_stats_player_id_won",
                table: "player_match_stats",
                columns: new[] { "player_id", "won" });

            migrationBuilder.CreateIndex(
                name: "ix_player_osu_ruleset_data_player_id_ruleset",
                table: "player_osu_ruleset_data",
                columns: new[] { "player_id", "ruleset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_ratings_player_id",
                table: "player_ratings",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_ratings_player_id_ruleset",
                table: "player_ratings",
                columns: new[] { "player_id", "ruleset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_ratings_rating",
                table: "player_ratings",
                column: "rating",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_player_ratings_ruleset",
                table: "player_ratings",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "ix_player_tournament_stats_player_id_tournament_id",
                table: "player_tournament_stats",
                columns: new[] { "player_id", "tournament_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_player_tournament_stats_tournament_id",
                table: "player_tournament_stats",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "ix_players_osu_id",
                table: "players",
                column: "osu_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rating_adjustments_match_id",
                table: "rating_adjustments",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "ix_rating_adjustments_player_id_match_id",
                table: "rating_adjustments",
                columns: new[] { "player_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rating_adjustments_player_id_timestamp",
                table: "rating_adjustments",
                columns: new[] { "player_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_rating_adjustments_player_rating_id",
                table: "rating_adjustments",
                column: "player_rating_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_admin_notes_admin_user_id",
                table: "tournament_admin_notes",
                column: "admin_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_admin_notes_reference_id",
                table: "tournament_admin_notes",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_audits_reference_id",
                table: "tournament_audits",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournaments_name_abbreviation",
                table: "tournaments",
                columns: new[] { "name", "abbreviation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tournaments_ruleset",
                table: "tournaments",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "ix_tournaments_submitted_by_user_id",
                table: "tournaments",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournaments_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_settings_user_id",
                table: "user_settings",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_player_id",
                table: "users",
                column: "player_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beatmap_attributes");

            migrationBuilder.DropTable(
                name: "game_admin_notes");

            migrationBuilder.DropTable(
                name: "game_audits");

            migrationBuilder.DropTable(
                name: "game_score_admin_notes");

            migrationBuilder.DropTable(
                name: "game_score_audits");

            migrationBuilder.DropTable(
                name: "game_win_records");

            migrationBuilder.DropTable(
                name: "join_beatmap_creators");

            migrationBuilder.DropTable(
                name: "join_pooled_beatmaps");

            migrationBuilder.DropTable(
                name: "match_admin_notes");

            migrationBuilder.DropTable(
                name: "match_audits");

            migrationBuilder.DropTable(
                name: "match_win_records");

            migrationBuilder.DropTable(
                name: "o_auth_client_admin_note");

            migrationBuilder.DropTable(
                name: "player_admin_notes");

            migrationBuilder.DropTable(
                name: "player_highest_ranks");

            migrationBuilder.DropTable(
                name: "player_match_stats");

            migrationBuilder.DropTable(
                name: "player_osu_ruleset_data");

            migrationBuilder.DropTable(
                name: "player_tournament_stats");

            migrationBuilder.DropTable(
                name: "rating_adjustments");

            migrationBuilder.DropTable(
                name: "tournament_admin_notes");

            migrationBuilder.DropTable(
                name: "tournament_audits");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "game_scores");

            migrationBuilder.DropTable(
                name: "o_auth_clients");

            migrationBuilder.DropTable(
                name: "player_ratings");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "beatmaps");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "beatmapsets");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
