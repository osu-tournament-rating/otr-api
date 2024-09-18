using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class _202409170001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "default_ruleset",
                table: "player_highest_ranks",
                newName: "ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_highest_ranks_player_id_default_ruleset",
                table: "player_highest_ranks",
                newName: "IX_player_highest_ranks_player_id_ruleset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "player_highest_ranks",
                newName: "default_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_highest_ranks_player_id_ruleset",
                table: "player_highest_ranks",
                newName: "IX_player_highest_ranks_player_id_default_ruleset");
        }
    }
}
