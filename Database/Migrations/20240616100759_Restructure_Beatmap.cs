using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Beatmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "games_beatmaps_id_fk",
                table: "games");

            migrationBuilder.DropPrimaryKey(
                name: "beatmaps_pk",
                table: "beatmaps");

            migrationBuilder.RenameColumn(
                name: "game_mode",
                table: "beatmaps",
                newName: "ruleset");

            migrationBuilder.RenameIndex(
                name: "beatmaps_beatmapid",
                table: "beatmaps",
                newName: "IX_beatmaps_beatmap_id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "beatmaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_beatmaps",
                table: "beatmaps",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_games_beatmaps_beatmap_id",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_beatmaps_beatmap_id",
                table: "games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_beatmaps",
                table: "beatmaps");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "beatmaps",
                newName: "game_mode");

            migrationBuilder.RenameIndex(
                name: "IX_beatmaps_beatmap_id",
                table: "beatmaps",
                newName: "beatmaps_beatmapid");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "beatmaps",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(
                name: "beatmaps_pk",
                table: "beatmaps",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "games_beatmaps_id_fk",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id");
        }
    }
}
