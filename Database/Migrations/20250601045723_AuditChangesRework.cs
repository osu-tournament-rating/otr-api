using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AuditChangesRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "tournament_audits",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "match_audits",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<int>(
                name: "play_mode",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "game_score_audits",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "game_audits",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

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

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "tournament_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "match_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "game_score_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "changes",
                table: "game_audits",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
