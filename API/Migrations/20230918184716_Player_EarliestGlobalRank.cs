using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Player_EarliestGlobalRank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
