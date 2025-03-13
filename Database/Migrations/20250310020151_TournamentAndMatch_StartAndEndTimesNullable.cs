using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TournamentAndMatch_StartAndEndTimesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "'2007-09-17T00:00:00'::timestamp");


            // apply data processors times fill logic or nullify
            migrationBuilder.Sql(@"
            update matches
            set start_time = (select min(start_time) from games where games.match_id = matches.id)
            where start_time = '2007-09-17T00:00:00'::timestamp and processing_status >= 1;
            update matches
            set end_time = (select max(end_time) from games where games.match_id = matches.id)
            where end_time = '2007-09-17T00:00:00'::timestamp and processing_status >= 1;

            update tournaments
            set start_time = (select min(start_time) from matches where matches.tournament_id = tournaments.id)
            where start_time = '2007-09-17T00:00:00'::timestamp and processing_status >= 2;
            update tournaments
            set end_time = (select max(end_time) from matches where matches.tournament_id = tournaments.id)
            where end_time = '2007-09-17T00:00:00'::timestamp and processing_status >= 2;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "'2007-09-17T00:00:00'::timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
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
        }
    }
}
