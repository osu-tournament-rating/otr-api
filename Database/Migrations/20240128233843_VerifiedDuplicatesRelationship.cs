#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class VerifiedDuplicatesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_duplicates_users_VerifierId",
                table: "match_duplicates");

            migrationBuilder.DropIndex(
                name: "IX_match_duplicates_VerifierId",
                table: "match_duplicates");

            migrationBuilder.DropColumn(
                name: "VerifierId",
                table: "match_duplicates");

            migrationBuilder.AddForeignKey(
                name: "FK_match_duplicates_users_id",
                table: "match_duplicates",
                column: "id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_duplicates_users_id",
                table: "match_duplicates");

            migrationBuilder.AddColumn<int>(
                name: "VerifierId",
                table: "match_duplicates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_duplicates_VerifierId",
                table: "match_duplicates",
                column: "VerifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_match_duplicates_users_VerifierId",
                table: "match_duplicates",
                column: "VerifierId",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
