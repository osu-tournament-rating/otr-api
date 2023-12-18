using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMatchFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "abbreviation",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "forum",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "mode",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "rank_range_lower_bound",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "team_size",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "tournament_name",
                table: "matches");

            migrationBuilder.AlterColumn<int>(
                name: "tournament_id",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tournament_id",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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
                name: "mode",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_range_lower_bound",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_size",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tournament_name",
                table: "matches",
                type: "text",
                nullable: true);
        }
    }
}
