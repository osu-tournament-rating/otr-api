using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TournamentDefaultStartEndTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");
        }
    }
}
