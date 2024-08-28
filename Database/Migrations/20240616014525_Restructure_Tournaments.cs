using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Tournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "Tournaments_pk",
                table: "tournaments");
                //
            migrationBuilder.RenameColumn(
                name: "submitter_id",
                table: "tournaments",
                newName: "submitted_by_user_id");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "tournaments",
                newName: "ruleset");

            migrationBuilder.AddColumn<int>(
                name: "verified_by_user_id",
                table: "tournaments",
                type: "integer",
                nullable: true);

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_submitter_id",
                table: "tournaments",
                newName: "IX_tournaments_submitted_by_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournaments",
                table: "tournaments",
                column: "id");

            // migrationBuilder.CreateTable(
            //     name: "user_settings",
            //     columns: table => new
            //     {
            //         id = table.Column<int>(type: "integer", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
            //         created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
            //         updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //         user_id = table.Column<int>(type: "integer", nullable: false),
            //         default_ruleset = table.Column<int>(type: "integer", nullable: true),
            //         default_ruleset_controlled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("user_settings_pk", x => x.id);
            //         table.ForeignKey(
            //             name: "FK_user_settings_users_user_id",
            //             column: x => x.user_id,
            //             principalTable: "users",
            //             principalColumn: "id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_ruleset",
                table: "tournaments",
                column: "ruleset");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id");

            // migrationBuilder.CreateIndex(
            //     name: "IX_user_settings_user_id",
            //     table: "user_settings",
            //     column: "user_id",
            //     unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_tournaments_tournament_id",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_submitted_by_user_id",
                table: "tournaments",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_tournaments_tournament_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_submitted_by_user_id",
                table: "tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_verified_by_user_id",
                table: "tournaments");

            // migrationBuilder.DropTable(
            //     name: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournaments",
                table: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_tournaments_ruleset",
                table: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_tournaments_submitted_by_user_id",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "verified_by_user_id",
                table: "tournaments");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "tournaments",
                newName: "mode");

            migrationBuilder.RenameColumn(
                name: "submitted_by_user_id",
                table: "tournaments",
                newName: "submitter_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_submitted_by_user_id",
                table: "tournaments",
                newName: "IX_tournaments_submitter_id");

            migrationBuilder.AddPrimaryKey(
                name: "Tournaments_pk",
                table: "tournaments",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments",
                column: "submitter_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
