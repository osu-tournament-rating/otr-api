using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Player_OsuAPI_Data_ToJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "earliest_catch_global_rank",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_catch_global_rank_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_mania_global_rank",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_mania_global_rank_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_osu_global_rank",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_osu_global_rank_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_taiko_global_rank",
                table: "players");

            migrationBuilder.DropColumn(
                name: "earliest_taiko_global_rank_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "rank_catch",
                table: "players");

            migrationBuilder.DropColumn(
                name: "rank_mania",
                table: "players");

            migrationBuilder.DropColumn(
                name: "rank_standard",
                table: "players");

            migrationBuilder.DropColumn(
                name: "rank_taiko",
                table: "players");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "players",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "default_ruleset",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "players",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "osu_last_fetch",
                table: "players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<DateTime>(
                name: "osu_track_last_fetch",
                table: "players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<string>(
                name: "ruleset_data",
                table: "players",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "osu_last_fetch",
                table: "players");

            migrationBuilder.DropColumn(
                name: "osu_track_last_fetch",
                table: "players");

            migrationBuilder.DropColumn(
                name: "ruleset_data",
                table: "players");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "players",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<int>(
                name: "default_ruleset",
                table: "players",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "players",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4);

            migrationBuilder.AddColumn<int>(
                name: "earliest_catch_global_rank",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "earliest_catch_global_rank_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "earliest_mania_global_rank",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "earliest_mania_global_rank_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "earliest_osu_global_rank",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "earliest_osu_global_rank_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "earliest_taiko_global_rank",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "earliest_taiko_global_rank_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_catch",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_mania",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_standard",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_taiko",
                table: "players",
                type: "integer",
                nullable: true);
        }
    }
}
