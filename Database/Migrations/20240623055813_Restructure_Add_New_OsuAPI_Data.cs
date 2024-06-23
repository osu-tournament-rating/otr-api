using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Add_New_OsuAPI_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aim_diff",
                table: "beatmaps");

            migrationBuilder.DropColumn(
                name: "drain_time",
                table: "beatmaps");

            migrationBuilder.DropColumn(
                name: "speed_diff",
                table: "beatmaps");

            migrationBuilder.RenameColumn(
                name: "beatmap_id",
                table: "beatmaps",
                newName: "osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmaps_beatmap_id",
                table: "beatmaps",
                newName: "IX_beatmaps_osu_id");

            migrationBuilder.AddColumn<int>(
                name: "grade",
                table: "game_scores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "max_combo",
                table: "beatmaps",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "diff_name",
                table: "beatmaps",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_data",
                table: "beatmaps",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "ranked_status",
                table: "beatmaps",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grade",
                table: "game_scores");

            migrationBuilder.DropColumn(
                name: "has_data",
                table: "beatmaps");

            migrationBuilder.DropColumn(
                name: "ranked_status",
                table: "beatmaps");

            migrationBuilder.RenameColumn(
                name: "osu_id",
                table: "beatmaps",
                newName: "beatmap_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmaps_osu_id",
                table: "beatmaps",
                newName: "IX_beatmaps_beatmap_id");

            migrationBuilder.AlterColumn<int>(
                name: "max_combo",
                table: "beatmaps",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "diff_name",
                table: "beatmaps",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<double>(
                name: "aim_diff",
                table: "beatmaps",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "drain_time",
                table: "beatmaps",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "speed_diff",
                table: "beatmaps",
                type: "double precision",
                nullable: true);
        }
    }
}
