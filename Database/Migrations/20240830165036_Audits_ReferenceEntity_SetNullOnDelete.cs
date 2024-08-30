using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Audits_ReferenceEntity_SetNullOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits");

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "tournament_audits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ref_id_lock",
                table: "tournament_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "match_audits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ref_id_lock",
                table: "match_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "game_score_audits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ref_id_lock",
                table: "game_score_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "game_audits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ref_id_lock",
                table: "game_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits",
                column: "ref_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits",
                column: "ref_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits",
                column: "ref_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits",
                column: "ref_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits");

            migrationBuilder.DropColumn(
                name: "ref_id_lock",
                table: "tournament_audits");

            migrationBuilder.DropColumn(
                name: "ref_id_lock",
                table: "match_audits");

            migrationBuilder.DropColumn(
                name: "ref_id_lock",
                table: "game_score_audits");

            migrationBuilder.DropColumn(
                name: "ref_id_lock",
                table: "game_audits");

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "tournament_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "match_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "game_score_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ref_id",
                table: "game_audits",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits",
                column: "ref_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits",
                column: "ref_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits",
                column: "ref_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits",
                column: "ref_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
