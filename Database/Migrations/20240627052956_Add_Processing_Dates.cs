using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_Processing_Dates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_processing_date",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<double>(
                name: "bpm",
                table: "beatmaps",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "tournaments");

            migrationBuilder.DropColumn(
                name: "last_processing_date",
                table: "matches");

            migrationBuilder.AlterColumn<double>(
                name: "bpm",
                table: "beatmaps",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
