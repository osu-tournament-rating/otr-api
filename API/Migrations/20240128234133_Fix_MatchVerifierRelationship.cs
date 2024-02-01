using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Fix_MatchVerifierRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_duplicates_users_id",
                table: "match_duplicates");

            migrationBuilder.CreateIndex(
                name: "IX_match_duplicates_verified_by",
                table: "match_duplicates",
                column: "verified_by");

            migrationBuilder.AddForeignKey(
                name: "FK_match_duplicates_users_verified_by",
                table: "match_duplicates",
                column: "verified_by",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_duplicates_users_verified_by",
                table: "match_duplicates");

            migrationBuilder.DropIndex(
                name: "IX_match_duplicates_verified_by",
                table: "match_duplicates");

            migrationBuilder.AddForeignKey(
                name: "FK_match_duplicates_users_id",
                table: "match_duplicates",
                column: "id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
