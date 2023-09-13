using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class NullableTypes_Beatmap_CompositeKey_BeatmapModSr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "beatmap_mod_sr_pk",
                table: "beatmap_mod_sr");

            migrationBuilder.AlterColumn<int>(
                name: "max_combo",
                table: "beatmaps",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "bpm",
                table: "beatmaps",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddPrimaryKey(
                name: "PK_beatmap_mod_sr",
                table: "beatmap_mod_sr",
                columns: new[] { "beatmap_id", "mods" });

            migrationBuilder.CreateIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr",
                column: "beatmap_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_beatmap_mod_sr",
                table: "beatmap_mod_sr");

            migrationBuilder.DropIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr");

            migrationBuilder.AlterColumn<int>(
                name: "max_combo",
                table: "beatmaps",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "bpm",
                table: "beatmaps",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "beatmap_mod_sr_pk",
                table: "beatmap_mod_sr",
                column: "beatmap_id");
        }
    }
}
