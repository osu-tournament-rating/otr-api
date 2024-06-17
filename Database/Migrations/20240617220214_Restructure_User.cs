using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Users___fkplayerid",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "User_pk",
                table: "users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_players_player_id",
                table: "users",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_players_player_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.AddPrimaryKey(
                name: "User_pk",
                table: "users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "Users___fkplayerid",
                table: "users",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");
        }
    }
}
