using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class PlayerHighestRanks : Migration
    {
        private static readonly string[] columns = new[] { "player_id", "ruleset" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_highest_ranks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    ruleset = table.Column<int>(type: "integer", nullable: false),
                    global_rank = table.Column<int>(type: "integer", nullable: false),
                    global_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    country_rank = table.Column<int>(type: "integer", nullable: false),
                    country_rank_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_highest_ranks", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_highest_ranks_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_highest_ranks_country_rank",
                table: "player_highest_ranks",
                column: "country_rank",
                descending: Array.Empty<bool>());

            migrationBuilder.CreateIndex(
                name: "IX_player_highest_ranks_global_rank",
                table: "player_highest_ranks",
                column: "global_rank",
                descending: Array.Empty<bool>());

            migrationBuilder.CreateIndex(
                name: "IX_player_highest_ranks_player_id_ruleset",
                table: "player_highest_ranks",
                columns: columns,
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_highest_ranks");
        }
    }
}
