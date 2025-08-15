﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProcessingStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "processing_status",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "processing_status",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "games");

            migrationBuilder.DropColumn(
                name: "processing_status",
                table: "games");

            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "game_scores");

            migrationBuilder.DropColumn(
                name: "processing_status",
                table: "game_scores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<int>(
                name: "processing_status",
                table: "tournaments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<int>(
                name: "processing_status",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<int>(
                name: "processing_status",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "game_scores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<int>(
                name: "processing_status",
                table: "game_scores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
