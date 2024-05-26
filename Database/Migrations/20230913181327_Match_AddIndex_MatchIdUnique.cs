#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Match_AddIndex_MatchIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_matches_match_id",
                table: "matches",
                column: "match_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_matches_match_id",
                table: "matches");
        }
    }
}
