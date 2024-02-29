using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Match_Remove_SubmittedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_submitter_id",
                table: "tournaments",
                column: "submitter_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments",
                column: "submitter_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_tournaments_submitter_id",
                table: "tournaments");

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
    }
}
