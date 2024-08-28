using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_MatchAudits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_api_processed",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "needs_auto_check",
                table: "matches");

            migrationBuilder.RenameColumn(
                name: "match_id",
                table: "matches",
                newName: "osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_match_id",
                table: "matches",
                newName: "IX_matches_osu_id");

            migrationBuilder.AlterColumn<int>(
                name: "verification_status",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "processing_status",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rejection_reason",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "match_audits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ref_id = table.Column<int>(type: "integer", nullable: false),
                    action_user_id = table.Column<int>(type: "integer", nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_audits_ref_id",
                table: "match_audits",
                column: "ref_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_audits");

            migrationBuilder.DropColumn(
                name: "processing_status",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "matches");

            migrationBuilder.RenameColumn(
                name: "osu_id",
                table: "matches",
                newName: "match_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_osu_id",
                table: "matches",
                newName: "IX_matches_match_id");

            migrationBuilder.AlterColumn<int>(
                name: "verification_status",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "is_api_processed",
                table: "matches",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "needs_auto_check",
                table: "matches",
                type: "boolean",
                nullable: true);
        }
    }
}
