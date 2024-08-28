using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_PlayerTournamentStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_tournament_stats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    average_rating_delta = table.Column<double>(type: "double precision", nullable: false),
                    average_score = table.Column<int>(type: "integer", nullable: false),
                    average_accuracy = table.Column<double>(type: "double precision", nullable: false),
                    average_match_cost = table.Column<double>(type: "double precision", nullable: false),
                    average_placement = table.Column<double>(type: "double precision", nullable: false),
                    matches_played = table.Column<int>(type: "integer", nullable: false),
                    matches_won = table.Column<int>(type: "integer", nullable: false),
                    matches_lost = table.Column<int>(type: "integer", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    games_won = table.Column<int>(type: "integer", nullable: false),
                    games_lost = table.Column<int>(type: "integer", nullable: false),
                    teammate_ids = table.Column<int[]>(type: "integer[]", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_tournament_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_tournament_stats_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_player_tournament_stats_tournaments_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_tournament_stats_player_id_tournament_id",
                table: "player_tournament_stats",
                columns: new[] { "player_id", "tournament_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_tournament_stats_tournament_id",
                table: "player_tournament_stats",
                column: "tournament_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_tournament_stats");
        }
    }
}
