using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Move_OsuRulesetData_ToOwnTable : Migration
    {
        private static readonly string[] columns = ["player_id", "ruleset"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ruleset_data",
                table: "players");

            migrationBuilder.CreateTable(
                name: "player_osu_ruleset_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    pp = table.Column<double>(type: "double precision", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    earliest_global_rank = table.Column<int>(type: "integer", nullable: true),
                    earliest_global_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_osu_ruleset_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_osu_ruleset_data_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_osu_ruleset_data_player_id_ruleset",
                table: "player_osu_ruleset_data",
                columns: columns,
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_osu_ruleset_data");

            migrationBuilder.AddColumn<string>(
                name: "ruleset_data",
                table: "players",
                type: "jsonb",
                nullable: true);
        }
    }
}
