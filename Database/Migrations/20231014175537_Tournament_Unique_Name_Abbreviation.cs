#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Tournament_Unique_Name_Abbreviation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tournaments_name_abbreviation",
                table: "tournaments",
                columns: new[] { "name", "abbreviation" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tournaments_name_abbreviation",
                table: "tournaments");
        }
    }
}
