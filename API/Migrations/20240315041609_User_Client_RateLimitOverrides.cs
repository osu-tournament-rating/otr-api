using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class User_Client_RateLimitOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "rate_limit_overrides",
                table: "users",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rate_limit_overrides",
                table: "oauth_clients",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rate_limit_overrides",
                table: "users");

            migrationBuilder.DropColumn(
                name: "rate_limit_overrides",
                table: "oauth_clients");
        }
    }
}
