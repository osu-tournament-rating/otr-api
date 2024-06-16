using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Match : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "match_win_records_matches_id_fk",
                table: "match_win_records");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_submitted_by_user",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_verified_by_user",
                table: "matches");

            migrationBuilder.Sql(
                """
                ALTER TABLE matches RENAME CONSTRAINT "matches_pk" TO "PK_matches"
                """
            );

            migrationBuilder.DropIndex(
                name: "osumatches_matchid",
                table: "matches");

            // Manual
            migrationBuilder.DropColumn(
                name: "verification_source",
                table: "matches");

            // Manual
            migrationBuilder.DropColumn(
                name: "verification_info",
                table: "matches");

            // Manual
            migrationBuilder.RenameColumn(
                name: "submitted_by_user",
                table: "matches",
                newName: "submitted_by_user_id");

            // Manual
            migrationBuilder.RenameColumn(
                name: "verified_by_user",
                table: "matches",
                newName: "verified_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_verified_by_user",
                table: "matches",
                newName: "IX_matches_verified_by_user_id");

            // Manual
            migrationBuilder.RenameIndex(
                name: "IX_matches_submitted_by_user",
                table: "matches",
                newName: "IX_matches_submitted_by_user_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "matches",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "matches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_submitted_by_user_id",
                table: "matches",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_verified_by_user_id",
                table: "matches",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_submitted_by_user_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_verified_by_user_id",
                table: "matches");

            migrationBuilder.Sql(
                """
                ALTER TABLE matches RENAME CONSTRAINT "PK_matches" TO "matches_pk"
                """
            );

            migrationBuilder.RenameColumn(
                name: "verified_by_user_id",
                table: "matches",
                newName: "verified_by_user");

            migrationBuilder.RenameIndex(
                name: "IX_matches_verified_by_user_id",
                table: "matches",
                newName: "IX_matches_verified_by_user");

            // Manual
            migrationBuilder.RenameIndex(
                name: "IX_matches_submitted_by_user_id",
                table: "matches",
                newName: "IX_matches_submitted_by_user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "matches",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "matches",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<int>(
                name: "verification_source",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verification_info",
                table: "matches",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            // Manual
            migrationBuilder.RenameColumn(
                name: "submitted_by_user_id",
                table: "matches",
                newName: "submitted_by_user");

            migrationBuilder.CreateIndex(
                name: "osumatches_matchid",
                table: "matches",
                column: "match_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "match_win_records_matches_id_fk",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_submitted_by_user",
                table: "matches",
                column: "submitted_by_user",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_verified_by_user",
                table: "matches",
                column: "verified_by_user",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
