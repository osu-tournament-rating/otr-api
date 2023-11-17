using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Remove_RatingHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ratinghistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ratinghistories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    match_id = table.Column<int>(type: "integer", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    mu = table.Column<double>(type: "double precision", nullable: false),
                    sigma = table.Column<double>(type: "double precision", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("RatingHistories_pk", x => x.id);
                    table.ForeignKey(
                        name: "RatingHistories___fkplayerid",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ratinghistories_matches_id_fk",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_match_id",
                table: "ratinghistories",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratinghistories_player_id",
                table: "ratinghistories",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ratinghistories_pk",
                table: "ratinghistories",
                columns: new[] { "player_id", "match_id" },
                unique: true);
        }
    }
}
