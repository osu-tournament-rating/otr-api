#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Tournament_Add_SubmitterUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "submitter_id",
                table: "tournaments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "submitter_id",
                table: "tournaments");
        }
    }
}
