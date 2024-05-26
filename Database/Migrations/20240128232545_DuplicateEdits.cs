#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class DuplicateEdits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_match_duplicate_xref_matches_match_id",
                table: "match_duplicate_xref");

            migrationBuilder.RenameTable(
                name: "match_duplicate_xref",
                newName: "match_duplicates");

            migrationBuilder.RenameColumn(
                name: "match_id",
                table: "match_duplicates",
                newName: "suspected_duplicate_of");

            migrationBuilder.RenameIndex(
                name: "IX_match_duplicate_xref_osu_match_id",
                table: "match_duplicates",
                newName: "IX_match_duplicates_osu_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_duplicate_xref_match_id",
                table: "match_duplicates",
                newName: "IX_match_duplicates_suspected_duplicate_of");

            migrationBuilder.AddColumn<int>(
                name: "VerifierId",
                table: "match_duplicates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "verified_by",
                table: "match_duplicates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "verified_duplicate",
                table: "match_duplicates",
                type: "boolean",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "verified_by",
                table: "match_duplicates");

            migrationBuilder.DropColumn(
                name: "verified_duplicate",
                table: "match_duplicates");

            migrationBuilder.RenameTable(
                name: "match_duplicates",
                newName: "match_duplicate_xref");

            migrationBuilder.RenameColumn(
                name: "suspected_duplicate_of",
                table: "match_duplicate_xref",
                newName: "match_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_duplicates_suspected_duplicate_of",
                table: "match_duplicate_xref",
                newName: "IX_match_duplicate_xref_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_duplicates_osu_match_id",
                table: "match_duplicate_xref",
                newName: "IX_match_duplicate_xref_osu_match_id");

            migrationBuilder.AddForeignKey(
                name: "FK_match_duplicate_xref_matches_match_id",
                table: "match_duplicate_xref",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
