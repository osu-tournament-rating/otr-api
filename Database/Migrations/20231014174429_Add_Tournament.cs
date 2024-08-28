#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tournament_id",
                table: "matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    abbreviation = table.Column<string>(type: "text", nullable: false),
                    forum_url = table.Column<string>(type: "text", nullable: false),
                    rank_range_lower_bound = table.Column<int>(type: "integer", nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    team_size = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Tournaments_pk", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_tournament_id",
                table: "matches",
                column: "tournament_id");

            migrationBuilder.AddForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Tournaments___fkmatchid",
                table: "matches");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_matches_tournament_id",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "tournament_id",
                table: "matches");
        }
    }
}
