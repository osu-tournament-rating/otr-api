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

            migrationBuilder.Sql(
                """
                ALTER TABLE users RENAME CONSTRAINT "User_pk" TO "PK_users"
                """
            );

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

            migrationBuilder.Sql(
                """
                ALTER TABLE users RENAME CONSTRAINT "PK_users" TO "User_pk"
                """
            );

            migrationBuilder.AddForeignKey(
                name: "Users___fkplayerid",
                table: "users",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id");
        }
    }
}
