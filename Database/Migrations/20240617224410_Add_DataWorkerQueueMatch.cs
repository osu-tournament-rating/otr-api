using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_DataWorkerQueueMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_worker_queue_matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    osu_match_id = table.Column<long>(type: "bigint", nullable: false),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_worker_queue_matches", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_data_worker_queue_matches_osu_match_id",
                table: "data_worker_queue_matches",
                column: "osu_match_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_worker_queue_matches");
        }
    }
}
