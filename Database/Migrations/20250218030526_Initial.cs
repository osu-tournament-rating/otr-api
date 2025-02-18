using System;
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
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OsuId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: ""),
                    Country = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false, defaultValue: ""),
                    Ruleset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    OsuLastFetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    OsuTrackLastFetch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beatmapsets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OsuId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: true),
                    Artist = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    RankedStatus = table.Column<int>(type: "integer", nullable: false),
                    RankedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmapsets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beatmapsets_Players_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerHighestRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    GlobalRank = table.Column<int>(type: "integer", nullable: false),
                    GlobalRankDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CountryRank = table.Column<int>(type: "integer", nullable: false),
                    CountryRankDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHighestRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerHighestRanks_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerOsuRulesetData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    Pp = table.Column<double>(type: "double precision", nullable: false),
                    GlobalRank = table.Column<int>(type: "integer", nullable: false),
                    EarliestGlobalRank = table.Column<int>(type: "integer", nullable: true),
                    EarliestGlobalRankDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerOsuRulesetData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerOsuRulesetData_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Volatility = table.Column<double>(type: "double precision", nullable: false),
                    Percentile = table.Column<double>(type: "double precision", nullable: false),
                    GlobalRank = table.Column<int>(type: "integer", nullable: false),
                    CountryRank = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRatings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Scopes = table.Column<string[]>(type: "text[]", nullable: false, defaultValue: Array.Empty<string>()),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Beatmaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OsuId = table.Column<long>(type: "bigint", nullable: false),
                    HasData = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    RankedStatus = table.Column<int>(type: "integer", nullable: false),
                    DiffName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TotalLength = table.Column<long>(type: "bigint", nullable: false),
                    DrainLength = table.Column<int>(type: "integer", nullable: false),
                    Bpm = table.Column<double>(type: "double precision", nullable: false),
                    CountCircle = table.Column<int>(type: "integer", nullable: false),
                    CountSlider = table.Column<int>(type: "integer", nullable: false),
                    CountSpinner = table.Column<int>(type: "integer", nullable: false),
                    Cs = table.Column<double>(type: "double precision", nullable: false),
                    Hp = table.Column<double>(type: "double precision", nullable: false),
                    Od = table.Column<double>(type: "double precision", nullable: false),
                    Ar = table.Column<double>(type: "double precision", nullable: false),
                    Sr = table.Column<double>(type: "double precision", nullable: false),
                    MaxCombo = table.Column<int>(type: "integer", nullable: true),
                    BeatmapsetId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beatmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beatmaps_Beatmapsets_BeatmapsetId",
                        column: x => x.BeatmapsetId,
                        principalTable: "Beatmapsets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Scopes = table.Column<string[]>(type: "text[]", nullable: false),
                    RateLimitOverride = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthClients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerAdminNotes_Players_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerAdminNotes_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ForumUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RankRangeLowerBound = table.Column<int>(type: "integer", nullable: false),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    LobbySize = table.Column<int>(type: "integer", nullable: false),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    RejectionReason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SubmittedByUserId = table.Column<int>(type: "integer", nullable: true),
                    VerifiedByUserId = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tournaments_Users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    DefaultRuleset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    DefaultRulesetIsControlled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Mods = table.Column<int>(type: "integer", nullable: false),
                    Sr = table.Column<double>(type: "double precision", nullable: false),
                    BeatmapId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeatmapAttributes_Beatmaps_BeatmapId",
                        column: x => x.BeatmapId,
                        principalTable: "Beatmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinBeatmapCreators",
                columns: table => new
                {
                    CreatedBeatmapsId = table.Column<int>(type: "integer", nullable: false),
                    CreatorsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinBeatmapCreators", x => new { x.CreatedBeatmapsId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_JoinBeatmapCreators_Beatmaps_CreatedBeatmapsId",
                        column: x => x.CreatedBeatmapsId,
                        principalTable: "Beatmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinBeatmapCreators_Players_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthClientAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClientAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthClientAdminNotes_OAuthClients_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "OAuthClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinPooledBeatmaps",
                columns: table => new
                {
                    PooledBeatmapsId = table.Column<int>(type: "integer", nullable: false),
                    TournamentsPooledInId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinPooledBeatmaps", x => new { x.PooledBeatmapsId, x.TournamentsPooledInId });
                    table.ForeignKey(
                        name: "FK_JoinPooledBeatmaps_Beatmaps_PooledBeatmapsId",
                        column: x => x.PooledBeatmapsId,
                        principalTable: "Beatmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinPooledBeatmaps_Tournaments_TournamentsPooledInId",
                        column: x => x.TournamentsPooledInId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OsuId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, defaultValue: ""),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    RejectionReason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WarningFlags = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByUserId = table.Column<int>(type: "integer", nullable: true),
                    VerifiedByUserId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Matches_Users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTournamentStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    AverageRatingDelta = table.Column<double>(type: "double precision", nullable: false),
                    AverageMatchCost = table.Column<double>(type: "double precision", nullable: false),
                    AverageScore = table.Column<int>(type: "integer", nullable: false),
                    AveragePlacement = table.Column<double>(type: "double precision", nullable: false),
                    AverageAccuracy = table.Column<double>(type: "double precision", nullable: false),
                    MatchesPlayed = table.Column<int>(type: "integer", nullable: false),
                    MatchesWon = table.Column<int>(type: "integer", nullable: false),
                    MatchesLost = table.Column<int>(type: "integer", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    GamesWon = table.Column<int>(type: "integer", nullable: false),
                    GamesLost = table.Column<int>(type: "integer", nullable: false),
                    TeammateIds = table.Column<int[]>(type: "integer[]", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTournamentStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerTournamentStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTournamentStats_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentAdminNotes_Tournaments_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentAdminNotes_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReferenceIdLock = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    ActionUserId = table.Column<int>(type: "integer", nullable: true),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentAudits_Tournaments_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OsuId = table.Column<long>(type: "bigint", nullable: false),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    ScoringType = table.Column<int>(type: "integer", nullable: false),
                    TeamType = table.Column<int>(type: "integer", nullable: false),
                    Mods = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RejectionReason = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WarningFlags = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    BeatmapId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Beatmaps_BeatmapId",
                        column: x => x.BeatmapId,
                        principalTable: "Beatmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Games_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchAdminNotes_Matches_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchAdminNotes_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReferenceIdLock = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    ActionUserId = table.Column<int>(type: "integer", nullable: true),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchAudits_Matches_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MatchWinRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    WinnerRoster = table.Column<int[]>(type: "integer[]", nullable: false),
                    LoserRoster = table.Column<int[]>(type: "integer[]", nullable: false),
                    WinnerTeam = table.Column<int>(type: "integer", nullable: false),
                    LoserTeam = table.Column<int>(type: "integer", nullable: false),
                    WinnerScore = table.Column<int>(type: "integer", nullable: false),
                    LoserScore = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchWinRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchWinRecords_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMatchStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    MatchCost = table.Column<double>(type: "double precision", nullable: false),
                    AverageScore = table.Column<double>(type: "double precision", nullable: false),
                    AveragePlacement = table.Column<double>(type: "double precision", nullable: false),
                    AverageMisses = table.Column<double>(type: "double precision", nullable: false),
                    AverageAccuracy = table.Column<double>(type: "double precision", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    GamesWon = table.Column<int>(type: "integer", nullable: false),
                    GamesLost = table.Column<int>(type: "integer", nullable: false),
                    Won = table.Column<bool>(type: "boolean", nullable: false),
                    TeammateIds = table.Column<int[]>(type: "integer[]", nullable: false),
                    OpponentIds = table.Column<int[]>(type: "integer[]", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RatingAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    AdjustmentType = table.Column<int>(type: "integer", nullable: false),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RatingBefore = table.Column<double>(type: "double precision", nullable: false),
                    RatingAfter = table.Column<double>(type: "double precision", nullable: false),
                    VolatilityBefore = table.Column<double>(type: "double precision", nullable: false),
                    VolatilityAfter = table.Column<double>(type: "double precision", nullable: false),
                    PlayerRatingId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingAdjustments_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatingAdjustments_PlayerRatings_PlayerRatingId",
                        column: x => x.PlayerRatingId,
                        principalTable: "PlayerRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatingAdjustments_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameAdminNotes_Games_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameAdminNotes_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReferenceIdLock = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    ActionUserId = table.Column<int>(type: "integer", nullable: true),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameAudits_Games_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GameScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Placement = table.Column<int>(type: "integer", nullable: false),
                    MaxCombo = table.Column<int>(type: "integer", nullable: false),
                    Count50 = table.Column<int>(type: "integer", nullable: false),
                    Count100 = table.Column<int>(type: "integer", nullable: false),
                    Count300 = table.Column<int>(type: "integer", nullable: false),
                    CountMiss = table.Column<int>(type: "integer", nullable: false),
                    CountKatu = table.Column<int>(type: "integer", nullable: false),
                    CountGeki = table.Column<int>(type: "integer", nullable: false),
                    Pass = table.Column<bool>(type: "boolean", nullable: false),
                    Perfect = table.Column<bool>(type: "boolean", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Mods = table.Column<int>(type: "integer", nullable: false),
                    Team = table.Column<int>(type: "integer", nullable: false),
                    Ruleset = table.Column<int>(type: "integer", nullable: false),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    LastProcessingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "'2007-09-17T00:00:00'::timestamp"),
                    RejectionReason = table.Column<int>(type: "integer", nullable: false),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScores_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScores_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameWinRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    WinnerRoster = table.Column<int[]>(type: "integer[]", nullable: false),
                    LoserRoster = table.Column<int[]>(type: "integer[]", nullable: false),
                    WinnerTeam = table.Column<int>(type: "integer", nullable: false),
                    LoserTeam = table.Column<int>(type: "integer", nullable: false),
                    WinnerScore = table.Column<int>(type: "integer", nullable: false),
                    LoserScore = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameWinRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameWinRecords_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameScoreAdminNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScoreAdminNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScoreAdminNotes_GameScores_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "GameScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameScoreAdminNotes_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameScoreAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReferenceIdLock = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    ActionUserId = table.Column<int>(type: "integer", nullable: true),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScoreAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScoreAudits_GameScores_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "GameScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapAttributes_BeatmapId_Mods",
                table: "BeatmapAttributes",
                columns: ["BeatmapId", "Mods"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beatmaps_BeatmapsetId",
                table: "Beatmaps",
                column: "BeatmapsetId");

            migrationBuilder.CreateIndex(
                name: "IX_Beatmaps_OsuId",
                table: "Beatmaps",
                column: "OsuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beatmapsets_CreatorId",
                table: "Beatmapsets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Beatmapsets_OsuId",
                table: "Beatmapsets",
                column: "OsuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameAdminNotes_AdminUserId",
                table: "GameAdminNotes",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameAdminNotes_ReferenceId",
                table: "GameAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_GameAudits_ReferenceId",
                table: "GameAudits",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_BeatmapId",
                table: "Games",
                column: "BeatmapId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_MatchId",
                table: "Games",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OsuId",
                table: "Games",
                column: "OsuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_StartTime",
                table: "Games",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_GameScoreAdminNotes_AdminUserId",
                table: "GameScoreAdminNotes",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScoreAdminNotes_ReferenceId",
                table: "GameScoreAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScoreAudits_ReferenceId",
                table: "GameScoreAudits",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_GameId",
                table: "GameScores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_PlayerId",
                table: "GameScores",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameScores_PlayerId_GameId",
                table: "GameScores",
                columns: ["PlayerId", "GameId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameWinRecords_GameId",
                table: "GameWinRecords",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameWinRecords_WinnerRoster",
                table: "GameWinRecords",
                column: "WinnerRoster");

            migrationBuilder.CreateIndex(
                name: "IX_JoinBeatmapCreators_CreatorsId",
                table: "JoinBeatmapCreators",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinPooledBeatmaps_TournamentsPooledInId",
                table: "JoinPooledBeatmaps",
                column: "TournamentsPooledInId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchAdminNotes_AdminUserId",
                table: "MatchAdminNotes",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchAdminNotes_ReferenceId",
                table: "MatchAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchAudits_ReferenceId",
                table: "MatchAudits",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_OsuId",
                table: "Matches",
                column: "OsuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SubmittedByUserId",
                table: "Matches",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TournamentId",
                table: "Matches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_VerifiedByUserId",
                table: "Matches",
                column: "VerifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchWinRecords_LoserRoster",
                table: "MatchWinRecords",
                column: "LoserRoster");

            migrationBuilder.CreateIndex(
                name: "IX_MatchWinRecords_MatchId",
                table: "MatchWinRecords",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchWinRecords_WinnerRoster",
                table: "MatchWinRecords",
                column: "WinnerRoster");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClientAdminNotes_ReferenceId",
                table: "OAuthClientAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClients_UserId",
                table: "OAuthClients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAdminNotes_AdminUserId",
                table: "PlayerAdminNotes",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAdminNotes_ReferenceId",
                table: "PlayerAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHighestRanks_CountryRank",
                table: "PlayerHighestRanks",
                column: "CountryRank",
                descending: []);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHighestRanks_GlobalRank",
                table: "PlayerHighestRanks",
                column: "GlobalRank",
                descending: []);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHighestRanks_PlayerId_Ruleset",
                table: "PlayerHighestRanks",
                columns: ["PlayerId", "Ruleset"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_MatchId",
                table: "PlayerMatchStats",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_PlayerId",
                table: "PlayerMatchStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_PlayerId_MatchId",
                table: "PlayerMatchStats",
                columns: ["PlayerId", "MatchId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchStats_PlayerId_Won",
                table: "PlayerMatchStats",
                columns: ["PlayerId", "Won"]);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerOsuRulesetData_PlayerId_Ruleset",
                table: "PlayerOsuRulesetData",
                columns: ["PlayerId", "Ruleset"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_PlayerId",
                table: "PlayerRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_PlayerId_Ruleset",
                table: "PlayerRatings",
                columns: ["PlayerId", "Ruleset"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_Rating",
                table: "PlayerRatings",
                column: "Rating",
                descending: []);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_Ruleset",
                table: "PlayerRatings",
                column: "Ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_Players_OsuId",
                table: "Players",
                column: "OsuId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTournamentStats_PlayerId_TournamentId",
                table: "PlayerTournamentStats",
                columns: ["PlayerId", "TournamentId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTournamentStats_TournamentId",
                table: "PlayerTournamentStats",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingAdjustments_MatchId",
                table: "RatingAdjustments",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingAdjustments_PlayerId_MatchId",
                table: "RatingAdjustments",
                columns: ["PlayerId", "MatchId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatingAdjustments_PlayerId_Timestamp",
                table: "RatingAdjustments",
                columns: ["PlayerId", "Timestamp"]);

            migrationBuilder.CreateIndex(
                name: "IX_RatingAdjustments_PlayerRatingId",
                table: "RatingAdjustments",
                column: "PlayerRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAdminNotes_AdminUserId",
                table: "TournamentAdminNotes",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAdminNotes_ReferenceId",
                table: "TournamentAdminNotes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentAudits_ReferenceId",
                table: "TournamentAudits",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_Name_Abbreviation",
                table: "Tournaments",
                columns: ["Name", "Abbreviation"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_Ruleset",
                table: "Tournaments",
                column: "Ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SubmittedByUserId",
                table: "Tournaments",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_VerifiedByUserId",
                table: "Tournaments",
                column: "VerifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PlayerId",
                table: "Users",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeatmapAttributes");

            migrationBuilder.DropTable(
                name: "GameAdminNotes");

            migrationBuilder.DropTable(
                name: "GameAudits");

            migrationBuilder.DropTable(
                name: "GameScoreAdminNotes");

            migrationBuilder.DropTable(
                name: "GameScoreAudits");

            migrationBuilder.DropTable(
                name: "GameWinRecords");

            migrationBuilder.DropTable(
                name: "JoinBeatmapCreators");

            migrationBuilder.DropTable(
                name: "JoinPooledBeatmaps");

            migrationBuilder.DropTable(
                name: "MatchAdminNotes");

            migrationBuilder.DropTable(
                name: "MatchAudits");

            migrationBuilder.DropTable(
                name: "MatchWinRecords");

            migrationBuilder.DropTable(
                name: "OAuthClientAdminNotes");

            migrationBuilder.DropTable(
                name: "PlayerAdminNotes");

            migrationBuilder.DropTable(
                name: "PlayerHighestRanks");

            migrationBuilder.DropTable(
                name: "PlayerMatchStats");

            migrationBuilder.DropTable(
                name: "PlayerOsuRulesetData");

            migrationBuilder.DropTable(
                name: "PlayerTournamentStats");

            migrationBuilder.DropTable(
                name: "RatingAdjustments");

            migrationBuilder.DropTable(
                name: "TournamentAdminNotes");

            migrationBuilder.DropTable(
                name: "TournamentAudits");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "GameScores");

            migrationBuilder.DropTable(
                name: "OAuthClients");

            migrationBuilder.DropTable(
                name: "PlayerRatings");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Beatmaps");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Beatmapsets");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
