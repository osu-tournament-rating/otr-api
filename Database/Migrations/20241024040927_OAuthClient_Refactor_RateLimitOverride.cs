using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class OAuthClient_Refactor_RateLimitOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rate_limit_overrides",
                table: "oauth_clients");

            migrationBuilder.AddColumn<int>(
                name: "rate_limit_override",
                table: "oauth_clients",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rate_limit_override",
                table: "oauth_clients");

            migrationBuilder.AddColumn<string>(
                name: "rate_limit_overrides",
                table: "oauth_clients",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
