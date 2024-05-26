#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchWinRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_win_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    team1 = table.Column<int[]>(type: "integer[]", nullable: false),
                    team2 = table.Column<int[]>(type: "integer[]", nullable: false),
                    team1_points = table.Column<int>(type: "integer", nullable: false),
                    team2_points = table.Column<int>(type: "integer", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: true),
                    loser_team = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_win_records_pk", x => x.id);
                    table.ForeignKey(
                        name: "match_win_records_matches_id_fk",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_match_id",
                table: "match_win_records",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_team1",
                table: "match_win_records",
                column: "team1");

            migrationBuilder.CreateIndex(
                name: "IX_match_win_records_team2",
                table: "match_win_records",
                column: "team2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_win_records");
        }
    }
}
