#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matches_hist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hist_ref_id = table.Column<int>(type: "integer", nullable: true),
                    hist_action = table.Column<int>(type: "integer", nullable: false),
                    hist_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hist_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    hist_modifier_id = table.Column<int>(type: "integer", nullable: true),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verification_info = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    verification_source = table.Column<int>(type: "integer", nullable: true),
                    verification_status = table.Column<int>(type: "integer", nullable: true),
                    verified_by_user = table.Column<int>(type: "integer", nullable: true),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    needs_auto_check = table.Column<bool>(type: "boolean", nullable: true),
                    is_api_processed = table.Column<bool>(type: "boolean", nullable: true),
                    submitted_by_user = table.Column<int>(type: "integer", nullable: true)
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
                name: "IX_matches_hist_hist_ref_id",
                table: "matches_hist",
                column: "hist_ref_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches_hist");
        }
    }
}
