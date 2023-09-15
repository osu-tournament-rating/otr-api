using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Match_SubmissionInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "abbreviation",
                table: "matches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "forum",
                table: "matches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "submitted_by",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "tournament_name",
                table: "matches",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "abbreviation",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "forum",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "submitted_by",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "tournament_name",
                table: "matches");
        }
    }
}
