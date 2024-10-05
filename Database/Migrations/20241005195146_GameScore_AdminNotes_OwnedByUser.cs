using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GameScore_AdminNotes_OwnedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_game_score_admin_notes_admin_user_id",
                table: "game_score_admin_notes",
                column: "admin_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes");

            migrationBuilder.DropIndex(
                name: "IX_game_score_admin_notes_admin_user_id",
                table: "game_score_admin_notes");
        }
    }
}
