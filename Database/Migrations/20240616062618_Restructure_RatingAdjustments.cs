using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_RatingAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "RatingAdjustment_pk",
                table: "rating_adjustments");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "rating_adjustments",
                newName: "ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_player_id_mode",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_player_id_ruleset");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "rating_adjustments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rating_adjustments",
                table: "rating_adjustments",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rating_adjustments",
                table: "rating_adjustments");

            migrationBuilder.DropColumn(
                name: "created",
                table: "rating_adjustments");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "rating_adjustments",
                newName: "mode");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_player_id_ruleset",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_player_id_mode");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "rating_adjustments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "RatingAdjustment_pk",
                table: "rating_adjustments",
                column: "id");
        }
    }
}
