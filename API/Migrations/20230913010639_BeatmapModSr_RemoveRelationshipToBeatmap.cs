using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class BeatmapModSr_RemoveRelationshipToBeatmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "beatmap_mod_sr_beatmaps_id_fk",
                table: "beatmap_mod_sr");

            migrationBuilder.DropIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr");

            migrationBuilder.CreateIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr",
                column: "beatmap_id");

            migrationBuilder.AddForeignKey(
                name: "FK_beatmap_mod_sr_beatmaps_beatmap_id",
                table: "beatmap_mod_sr",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_beatmap_mod_sr_beatmaps_beatmap_id",
                table: "beatmap_mod_sr");

            migrationBuilder.DropIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr");

            migrationBuilder.CreateIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr",
                column: "beatmap_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "beatmap_mod_sr_beatmaps_id_fk",
                table: "beatmap_mod_sr",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id");
        }
    }
}
