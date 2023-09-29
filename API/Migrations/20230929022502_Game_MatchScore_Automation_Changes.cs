using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Game_MatchScore_Automation_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_valid",
                table: "match_scores",
                type: "boolean",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "rejection_reason",
                table: "games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "verification_status",
                table: "games",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_valid",
                table: "match_scores");

            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "games");

            migrationBuilder.DropColumn(
                name: "verification_status",
                table: "games");
        }
    }
}
