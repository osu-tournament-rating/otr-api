using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_GameWinRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "game_win_records_games_id_fk",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "game_win_records_pk",
                table: "game_win_records");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "game_win_records",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "game_win_records",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_win_records",
                table: "game_win_records",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "games_game_win_records_id_fk",
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
                name: "games_game_win_records_id_fk",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_win_records",
                table: "game_win_records");

            migrationBuilder.DropColumn(
                name: "created",
                table: "game_win_records");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "game_win_records",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "game_win_records_pk",
                table: "game_win_records",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "game_win_records_games_id_fk",
                table: "game_win_records",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
