using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class key_renames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "BaseStatistics___fkplayerid",
                table: "base_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PlayerMatchStatistics_pk",
                table: "player_match_stats");

            migrationBuilder.DropPrimaryKey(
                name: "match_rating_statistics_pk",
                table: "match_rating_stats");

            migrationBuilder.DropPrimaryKey(
                name: "BaseStatistics_pk",
                table: "base_stats");

            migrationBuilder.AddPrimaryKey(
                name: "PlayerMatchStats_pk",
                table: "player_match_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "match_rating_stats_pk",
                table: "match_rating_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "BaseStats_pk",
                table: "base_stats",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "BaseStats___fkplayerid",
                table: "base_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PlayerMatchStats_pk",
                table: "player_match_stats");

            migrationBuilder.DropPrimaryKey(
                name: "match_rating_stats_pk",
                table: "match_rating_stats");

            migrationBuilder.DropPrimaryKey(
                name: "BaseStats_pk",
                table: "base_stats");

            migrationBuilder.AddPrimaryKey(
                name: "PlayerMatchStatistics_pk",
                table: "player_match_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "match_rating_statistics_pk",
                table: "match_rating_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "BaseStatistics_pk",
                table: "base_stats",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "BaseStatistics___fkplayerid",
                table: "base_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");
        }
    }
}
