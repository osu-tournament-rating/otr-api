using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Efficient_Indexes__PlayerTournamentStats_Add_MatchWinRate : Migration
    {
        private static readonly string[] columns = new[] { "ruleset", "rating" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "match_win_rate",
                table: "player_tournament_stats",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "ix_players_country",
                table: "players",
                column: "country");

            migrationBuilder.CreateIndex(
                name: "ix_player_ratings_ruleset_rating",
                table: "player_ratings",
                columns: columns,
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_player_osu_ruleset_data_player_id_ruleset_global_rank",
                table: "player_osu_ruleset_data",
                columns: new[] { "player_id", "ruleset", "global_rank" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_players_country",
                table: "players");

            migrationBuilder.DropIndex(
                name: "ix_player_ratings_ruleset_rating",
                table: "player_ratings");

            migrationBuilder.DropIndex(
                name: "ix_player_osu_ruleset_data_player_id_ruleset_global_rank",
                table: "player_osu_ruleset_data");

            migrationBuilder.DropColumn(
                name: "match_win_rate",
                table: "player_tournament_stats");
        }
    }
}
