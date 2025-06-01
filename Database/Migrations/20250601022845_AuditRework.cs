using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AuditRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changes",
                table: "tournament_audits");

            migrationBuilder.DropColumn(
                name: "changes",
                table: "match_audits");

            migrationBuilder.DropColumn(
                name: "changes",
                table: "game_score_audits");

            migrationBuilder.DropColumn(
                name: "changes",
                table: "game_audits");

            migrationBuilder.AddColumn<string>(
                name: "after",
                table: "tournament_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "before",
                table: "tournament_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "after",
                table: "match_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "before",
                table: "match_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "after",
                table: "game_score_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "before",
                table: "game_score_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "after",
                table: "game_audits",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "before",
                table: "game_audits",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "after",
                table: "tournament_audits");

            migrationBuilder.DropColumn(
                name: "before",
                table: "tournament_audits");

            migrationBuilder.DropColumn(
                name: "after",
                table: "match_audits");

            migrationBuilder.DropColumn(
                name: "before",
                table: "match_audits");

            migrationBuilder.DropColumn(
                name: "after",
                table: "game_score_audits");

            migrationBuilder.DropColumn(
                name: "before",
                table: "game_score_audits");

            migrationBuilder.DropColumn(
                name: "after",
                table: "game_audits");

            migrationBuilder.DropColumn(
                name: "before",
                table: "game_audits");

            migrationBuilder.AddColumn<string>(
                name: "changes",
                table: "tournament_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "changes",
                table: "match_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "changes",
                table: "game_score_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "changes",
                table: "game_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
