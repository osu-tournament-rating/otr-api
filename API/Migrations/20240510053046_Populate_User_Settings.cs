using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Populate_User_Settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Populate user settings with the default ruleset of their respective player
            // entry by default and use Ruleset.Standard as a fallback
            migrationBuilder.Sql(
                @"
                INSERT INTO user_settings (user_id, default_ruleset)
                SELECT u.id, coalesce(p.default_ruleset, 0)
                FROM users u
                JOIN players p ON p.id = u.player_id
                "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE user_settings");
        }
    }
}
