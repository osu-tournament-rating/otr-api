using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Games_Rename_GameId_To_OsuId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "games",
                newName: "osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_games_game_id",
                table: "games",
                newName: "IX_games_osu_id");

            migrationBuilder.AlterColumn<int>(
                name: "verification_status",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "rejection_reason",
                table: "games",
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
            migrationBuilder.RenameColumn(
                name: "osu_id",
                table: "games",
                newName: "game_id");

            migrationBuilder.RenameIndex(
                name: "IX_games_osu_id",
                table: "games",
                newName: "IX_games_game_id");

            migrationBuilder.AlterColumn<int>(
                name: "verification_status",
                table: "games",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "rejection_reason",
                table: "games",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
