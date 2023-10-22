using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Stats_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_game_statistics");

            migrationBuilder.AddColumn<double>(
                name: "average_opponent_rating",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "average_teammate_rating",
                table: "player_match_statistics",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int[]>(
                name: "opponent_ids",
                table: "player_match_statistics",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "teammate_ids",
                table: "player_match_statistics",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "average_opponent_rating",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "average_teammate_rating",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "opponent_ids",
                table: "player_match_statistics");

            migrationBuilder.DropColumn(
                name: "teammate_ids",
                table: "player_match_statistics");

            migrationBuilder.CreateTable(
                name: "player_game_statistics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    average_opponent_rating = table.Column<double>(type: "double precision", nullable: false),
                    average_teammate_rating = table.Column<double>(type: "double precision", nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    opponent_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    placing = table.Column<int>(type: "integer", nullable: false),
                    played_dt = table.Column<bool>(type: "boolean", nullable: false),
                    played_hd = table.Column<bool>(type: "boolean", nullable: false),
                    played_hr = table.Column<bool>(type: "boolean", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    won = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PlayerGameStatistics_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_game_statistics_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "games_player_statistics_game_id_fk",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_game_statistics_game_id",
                table: "player_game_statistics",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_game_statistics_player_id",
                table: "player_game_statistics",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_game_statistics_player_id_game_id",
                table: "player_game_statistics",
                columns: new[] { "player_id", "game_id" },
                unique: true);
        }
    }
}
