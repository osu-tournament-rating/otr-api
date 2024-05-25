#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class BeatmapModSr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeatmapModSr",
                columns: table => new
                {
                    beatmap_id = table.Column<int>(type: "integer", nullable: false),
                    post_mod_sr = table.Column<double>(type: "double precision", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("beatmap_mod_sr_pk", x => x.beatmap_id);
                    table.ForeignKey(
                        name: "beatmap_mod_sr_beatmaps_id_fk",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeatmapModSr");
        }
    }
}
