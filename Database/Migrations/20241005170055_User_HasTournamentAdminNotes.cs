using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class User_HasTournamentAdminNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tournament_admin_notes_admin_user_id",
                table: "tournament_admin_notes",
                column: "admin_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes");

            migrationBuilder.DropIndex(
                name: "IX_tournament_admin_notes_admin_user_id",
                table: "tournament_admin_notes");
        }
    }
}
