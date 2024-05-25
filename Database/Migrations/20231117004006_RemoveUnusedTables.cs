#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beatmap_mod_sr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "beatmap_mod_sr",
                columns: table => new
                {
                    beatmap_id = table.Column<int>(type: "integer", nullable: false),
                    mods = table.Column<int>(type: "integer", nullable: false),
                    post_mod_sr = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beatmap_mod_sr", x => new { x.beatmap_id, x.mods });
                    table.ForeignKey(
                        name: "FK_beatmap_mod_sr_beatmaps_beatmap_id",
                        column: x => x.beatmap_id,
                        principalTable: "beatmaps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beatmap_mod_sr_beatmap_id",
                table: "beatmap_mod_sr",
                column: "beatmap_id");
        }
    }
}
