using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTableIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "play_mode",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tournament_audits_action_user_id",
                table: "tournament_audits",
                column: "action_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_audits_action_user_id_created",
                table: "tournament_audits",
                columns: new[] { "action_user_id", "created" });

            migrationBuilder.CreateIndex(
                name: "ix_tournament_audits_created",
                table: "tournament_audits",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_tournament_audits_reference_id_lock",
                table: "tournament_audits",
                column: "reference_id_lock");

            migrationBuilder.CreateIndex(
                name: "ix_match_audits_action_user_id",
                table: "match_audits",
                column: "action_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_match_audits_action_user_id_created",
                table: "match_audits",
                columns: new[] { "action_user_id", "created" });

            migrationBuilder.CreateIndex(
                name: "ix_match_audits_created",
                table: "match_audits",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_match_audits_reference_id_lock",
                table: "match_audits",
                column: "reference_id_lock");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_audits_action_user_id",
                table: "game_score_audits",
                column: "action_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_audits_action_user_id_created",
                table: "game_score_audits",
                columns: new[] { "action_user_id", "created" });

            migrationBuilder.CreateIndex(
                name: "ix_game_score_audits_created",
                table: "game_score_audits",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_game_score_audits_reference_id_lock",
                table: "game_score_audits",
                column: "reference_id_lock");

            migrationBuilder.CreateIndex(
                name: "ix_game_audits_action_user_id",
                table: "game_audits",
                column: "action_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_audits_action_user_id_created",
                table: "game_audits",
                columns: new[] { "action_user_id", "created" });

            migrationBuilder.CreateIndex(
                name: "ix_game_audits_created",
                table: "game_audits",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "ix_game_audits_reference_id_lock",
                table: "game_audits",
                column: "reference_id_lock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tournament_audits_action_user_id",
                table: "tournament_audits");

            migrationBuilder.DropIndex(
                name: "ix_tournament_audits_action_user_id_created",
                table: "tournament_audits");

            migrationBuilder.DropIndex(
                name: "ix_tournament_audits_created",
                table: "tournament_audits");

            migrationBuilder.DropIndex(
                name: "ix_tournament_audits_reference_id_lock",
                table: "tournament_audits");

            migrationBuilder.DropIndex(
                name: "ix_match_audits_action_user_id",
                table: "match_audits");

            migrationBuilder.DropIndex(
                name: "ix_match_audits_action_user_id_created",
                table: "match_audits");

            migrationBuilder.DropIndex(
                name: "ix_match_audits_created",
                table: "match_audits");

            migrationBuilder.DropIndex(
                name: "ix_match_audits_reference_id_lock",
                table: "match_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_score_audits_action_user_id",
                table: "game_score_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_score_audits_action_user_id_created",
                table: "game_score_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_score_audits_created",
                table: "game_score_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_score_audits_reference_id_lock",
                table: "game_score_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_audits_action_user_id",
                table: "game_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_audits_action_user_id_created",
                table: "game_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_audits_created",
                table: "game_audits");

            migrationBuilder.DropIndex(
                name: "ix_game_audits_reference_id_lock",
                table: "game_audits");

            migrationBuilder.DropColumn(
                name: "play_mode",
                table: "games");
        }
    }
}
