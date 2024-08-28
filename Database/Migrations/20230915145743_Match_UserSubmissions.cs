#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Match_UserSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "submitted_by",
                table: "matches");

            migrationBuilder.AddColumn<int>(
                name: "submitted_by_user",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_matches_submitted_by_user",
                table: "matches",
                column: "submitted_by_user");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_submitted_by_user",
                table: "matches",
                column: "submitted_by_user",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_submitted_by_user",
                table: "matches");

            migrationBuilder.DropIndex(
                name: "IX_matches_submitted_by_user",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "submitted_by_user",
                table: "matches");

            migrationBuilder.AddColumn<int>(
                name: "submitted_by",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
