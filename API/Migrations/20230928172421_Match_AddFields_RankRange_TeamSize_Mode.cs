using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Match_AddFields_RankRange_TeamSize_Mode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "mode",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_range_lower_bound",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_size",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_username",
                table: "players",
                column: "username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_players_username",
                table: "players");

            migrationBuilder.DropColumn(
                name: "mode",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "rank_range_lower_bound",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "team_size",
                table: "matches");
        }
    }
}
