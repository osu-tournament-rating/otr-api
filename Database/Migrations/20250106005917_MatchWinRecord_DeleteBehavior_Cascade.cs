using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MatchWinRecord_DeleteBehavior_Cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records");

            migrationBuilder.AddForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records");

            migrationBuilder.AddForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
