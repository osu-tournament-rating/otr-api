using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Remove_MatchHistory_MatchDuplicates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_duplicates");

            migrationBuilder.DropTable(
                name: "matches_hist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_duplicates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    verified_by = table.Column<int>(type: "integer", nullable: true),
                    matchId = table.Column<int>(type: "integer", nullable: true),
                    osu_match_id = table.Column<long>(type: "bigint", nullable: false),
                    suspected_duplicate_of = table.Column<int>(type: "integer", nullable: false),
                    verified_duplicate = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_duplicate_xref_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_match_duplicates_users_verified_by",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "matches_hist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hist_ref_id = table.Column<int>(type: "integer", nullable: true),
                    hist_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hist_action = table.Column<int>(type: "integer", nullable: false),
                    hist_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    hist_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_api_processed = table.Column<bool>(type: "boolean", nullable: true),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    hist_modifier_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    needs_auto_check = table.Column<bool>(type: "boolean", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    submitted_by_user = table.Column<int>(type: "integer", nullable: true),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    verification_info = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    verification_source = table.Column<int>(type: "integer", nullable: true),
                    verification_status = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("matches_hist_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_hist_matches_hist_ref_id",
                        column: x => x.hist_ref_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_duplicates_osu_match_id",
                table: "match_duplicates",
                column: "osu_match_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_duplicates_suspected_duplicate_of",
                table: "match_duplicates",
                column: "suspected_duplicate_of");

            migrationBuilder.CreateIndex(
                name: "IX_match_duplicates_verified_by",
                table: "match_duplicates",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "IX_matches_hist_hist_ref_id",
                table: "matches_hist",
                column: "hist_ref_id");
        }
    }
}
