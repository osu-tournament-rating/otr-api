#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TournamentDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches");

            migrationBuilder.AddForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches");

            migrationBuilder.AddForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id");
        }
    }
}
