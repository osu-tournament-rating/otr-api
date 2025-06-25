using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FilterReportPlayers : Migration
    {
        private static readonly string[] columns = new[] { "filter_report_id", "player_id" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "filter_reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    min_rating = table.Column<int>(type: "integer", nullable: true),
                    max_rating = table.Column<int>(type: "integer", nullable: true),
                    tournaments_played = table.Column<int>(type: "integer", nullable: true),
                    peak_rating = table.Column<int>(type: "integer", nullable: true),
                    matches_played = table.Column<int>(type: "integer", nullable: true),
                    min_osu_rank = table.Column<int>(type: "integer", nullable: true),
                    max_osu_rank = table.Column<int>(type: "integer", nullable: true),
                    players_passed = table.Column<int>(type: "integer", nullable: false),
                    players_failed = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filter_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_filter_reports_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "filter_report_players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    filter_report_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    failure_reason = table.Column<int>(type: "integer", nullable: true),
                    current_rating = table.Column<double>(type: "double precision", nullable: true),
                    tournaments_played = table.Column<int>(type: "integer", nullable: true),
                    matches_played = table.Column<int>(type: "integer", nullable: true),
                    peak_rating = table.Column<double>(type: "double precision", nullable: true),
                    osu_global_rank = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_filter_report_players", x => x.id);
                    table.ForeignKey(
                        name: "fk_filter_report_players_filter_reports_filter_report_id",
                        column: x => x.filter_report_id,
                        principalTable: "filter_reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_filter_report_players_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_filter_report_players_filter_report_id",
                table: "filter_report_players",
                column: "filter_report_id");

            migrationBuilder.CreateIndex(
                name: "ix_filter_report_players_filter_report_id_player_id",
                table: "filter_report_players",
                columns: columns,
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_filter_report_players_player_id",
                table: "filter_report_players",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_filter_reports_user_id",
                table: "filter_reports",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filter_report_players");

            migrationBuilder.DropTable(
                name: "filter_reports");
        }
    }
}
