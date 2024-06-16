using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_MatchRatingStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "match_rating_stats_pk",
                table: "match_rating_stats");

            migrationBuilder.DropIndex(
                name: "IX_match_rating_stats_player_id",
                table: "match_rating_stats");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "match_rating_stats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "match_rating_stats",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_rating_stats",
                table: "match_rating_stats",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_stats_player_id_match_id",
                table: "match_rating_stats",
                columns: new[] { "player_id", "match_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_match_rating_stats",
                table: "match_rating_stats");

            migrationBuilder.DropIndex(
                name: "IX_match_rating_stats_player_id_match_id",
                table: "match_rating_stats");

            migrationBuilder.DropColumn(
                name: "created",
                table: "match_rating_stats");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "match_rating_stats",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "match_rating_stats_pk",
                table: "match_rating_stats",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_match_rating_stats_player_id",
                table: "match_rating_stats",
                column: "player_id");
        }
    }
}
