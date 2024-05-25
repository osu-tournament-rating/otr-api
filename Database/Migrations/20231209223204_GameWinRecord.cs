#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GameWinRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    winners = table.Column<int[]>(type: "integer[]", nullable: false),
                    losers = table.Column<int[]>(type: "integer[]", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: false),
                    loser_team = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("game_win_records_pk", x => x.id);
                    table.ForeignKey(
                        name: "game_win_records_games_id_fk",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_win_records_game_id",
                table: "game_win_records",
                column: "game_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_win_records_winners",
                table: "game_win_records",
                column: "winners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_win_records");
        }
    }
}
