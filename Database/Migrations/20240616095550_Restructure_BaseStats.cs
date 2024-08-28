using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_BaseStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats");

            migrationBuilder.DropForeignKey(
                name: "games_game_win_records_id_fk",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "BaseStats_pk",
                table: "base_stats");

            migrationBuilder.DropColumn(
                name: "updated",
                table: "base_stats");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "base_stats",
                newName: "ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_base_stats_player_id_mode",
                table: "base_stats",
                newName: "IX_base_stats_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_base_stats_mode",
                table: "base_stats",
                newName: "IX_base_stats_ruleset");

            migrationBuilder.AddPrimaryKey(
                name: "PK_base_stats",
                table: "base_stats",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_base_stats_players_player_id",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_win_records_games_game_id",
                table: "game_win_records",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_base_stats_players_player_id",
                table: "base_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_game_win_records_games_game_id",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_base_stats",
                table: "base_stats");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "base_stats",
                newName: "mode");

            migrationBuilder.RenameIndex(
                name: "IX_base_stats_ruleset",
                table: "base_stats",
                newName: "IX_base_stats_mode");

            migrationBuilder.RenameIndex(
                name: "IX_base_stats_player_id_ruleset",
                table: "base_stats",
                newName: "IX_base_stats_player_id_mode");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                table: "base_stats",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "BaseStats_pk",
                table: "base_stats",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "games_game_win_records_id_fk",
                table: "game_win_records",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
