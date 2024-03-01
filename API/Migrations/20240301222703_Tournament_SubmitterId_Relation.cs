using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Tournament_SubmitterId_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tournaments_submitter_id",
                table: "tournaments",
                column: "submitter_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments",
                column: "submitter_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_submitter_id",
                table: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_tournaments_submitter_id",
                table: "tournaments");
        }
    }
}
