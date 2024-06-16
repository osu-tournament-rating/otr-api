using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_MatchScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores");

            migrationBuilder.DropPrimaryKey(
                name: "match_scores_pk",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "match_scores_gameid_playerid",
                table: "match_scores");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "match_scores",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "match_scores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                table: "match_scores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_scores",
                table: "match_scores",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_game_id",
                table: "match_scores",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_scores_player_id_game_id",
                table: "match_scores",
                columns: new[] { "player_id", "game_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_match_scores_players_player_id",
                table: "match_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_scores_players_player_id",
                table: "match_scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_match_scores",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_game_id",
                table: "match_scores");

            migrationBuilder.DropIndex(
                name: "IX_match_scores_player_id_game_id",
                table: "match_scores");

            migrationBuilder.DropColumn(
                name: "created",
                table: "match_scores");

            migrationBuilder.DropColumn(
                name: "updated",
                table: "match_scores");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "match_scores",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "match_scores_pk",
                table: "match_scores",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "match_scores_gameid_playerid",
                table: "match_scores",
                columns: new[] { "game_id", "player_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "match_scores_players_id_fk",
                table: "match_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
