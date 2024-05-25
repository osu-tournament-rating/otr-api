#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class verificationprops : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApiProcessed",
                table: "matches",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "needs_auto_check",
                table: "matches",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "verified_by_user",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_matches_verified_by_user",
                table: "matches",
                column: "verified_by_user");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_verified_by_user",
                table: "matches",
                column: "verified_by_user",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_verified_by_user",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_matches_verified_by_user",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "IsApiProcessed",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "needs_auto_check",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "verified_by_user",
                table: "matches");
        }
    }
}
