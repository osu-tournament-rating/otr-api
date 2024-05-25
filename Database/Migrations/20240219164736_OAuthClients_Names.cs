#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class OAuthClients_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_oauth_clients_users_UserId",
                table: "oauth_clients");

            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "oauth_clients",
                newName: "secret");

            migrationBuilder.RenameColumn(
                name: "Scopes",
                table: "oauth_clients",
                newName: "scopes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "oauth_clients",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "oauth_clients",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_oauth_clients_UserId",
                table: "oauth_clients",
                newName: "IX_oauth_clients_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_oauth_clients_users_user_id",
                table: "oauth_clients",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_oauth_clients_users_user_id",
                table: "oauth_clients");

            migrationBuilder.RenameColumn(
                name: "secret",
                table: "oauth_clients",
                newName: "Secret");

            migrationBuilder.RenameColumn(
                name: "scopes",
                table: "oauth_clients",
                newName: "Scopes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "oauth_clients",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "oauth_clients",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_oauth_clients_user_id",
                table: "oauth_clients",
                newName: "IX_oauth_clients_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_oauth_clients_users_UserId",
                table: "oauth_clients",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
