using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Game : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "games_matches_id_fk",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores");

            migrationBuilder.Sql(
                """
                ALTER TABLE games RENAME CONSTRAINT "osugames_pk" TO "PK_games"
                """
            );

            migrationBuilder.DropIndex(
                name: "IX_games_game_id",
                table: "games");

            migrationBuilder.RenameIndex(
                name: "osugames_gameid",
                table: "games",
                newName: "IX_games_game_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "games",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_games_matches_match_id",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_match_scores_games_game_id",
                table: "match_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_matches_match_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "FK_match_scores_games_game_id",
                table: "match_scores");

            migrationBuilder.Sql(
                """
                ALTER TABLE games RENAME CONSTRAINT "PK_games" TO "osugames_pk"
                """
            );

            migrationBuilder.RenameIndex(
                name: "IX_games_game_id",
                table: "games",
                newName: "osugames_gameid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "games",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "games",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.CreateIndex(
                name: "IX_games_game_id",
                table: "games",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "games_matches_id_fk",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "match_scores_games_id_fk",
                table: "match_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
