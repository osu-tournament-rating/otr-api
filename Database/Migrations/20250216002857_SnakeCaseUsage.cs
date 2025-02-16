using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class SnakeCaseUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK___join__beatmap_creators_beatmaps_CreatedBeatmapsId",
                table: "__join__beatmap_creators");

            migrationBuilder.DropForeignKey(
                name: "FK___join__beatmap_creators_players_CreatorsId",
                table: "__join__beatmap_creators");

            migrationBuilder.DropForeignKey(
                name: "FK___join__pooled_beatmaps_beatmaps_PooledBeatmapsId",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropForeignKey(
                name: "FK___join__pooled_beatmaps_tournaments_TournamentsPooledInId",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropForeignKey(
                name: "FK_beatmap_attributes_beatmaps_beatmap_id",
                table: "beatmap_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_beatmaps_beatmapsets_beatmapset_id",
                table: "beatmaps");

            migrationBuilder.DropForeignKey(
                name: "FK_beatmapsets_players_creator_id",
                table: "beatmapsets");

            migrationBuilder.DropForeignKey(
                name: "FK_game_admin_notes_games_ref_id",
                table: "game_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_game_admin_notes_users_admin_user_id",
                table: "game_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_game_score_admin_notes_game_scores_ref_id",
                table: "game_score_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_game_scores_games_game_id",
                table: "game_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_game_scores_players_player_id",
                table: "game_scores");

            migrationBuilder.DropForeignKey(
                name: "FK_game_win_records_games_game_id",
                table: "game_win_records");

            migrationBuilder.DropForeignKey(
                name: "FK_games_beatmaps_beatmap_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "FK_games_matches_match_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "FK_match_admin_notes_matches_ref_id",
                table: "match_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_match_admin_notes_users_admin_user_id",
                table: "match_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_tournaments_tournament_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_submitted_by_user_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_matches_users_verified_by_user_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_oauth_client_admin_notes_oauth_clients_ref_id",
                table: "oauth_client_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_oauth_clients_users_user_id",
                table: "oauth_clients");

            migrationBuilder.DropForeignKey(
                name: "FK_player_admin_notes_players_ref_id",
                table: "player_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_player_admin_notes_users_admin_user_id",
                table: "player_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_player_highest_ranks_players_player_id",
                table: "player_highest_ranks");

            migrationBuilder.DropForeignKey(
                name: "FK_player_match_stats_matches_match_id",
                table: "player_match_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_player_match_stats_players_player_id",
                table: "player_match_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_player_osu_ruleset_data_players_player_id",
                table: "player_osu_ruleset_data");

            migrationBuilder.DropForeignKey(
                name: "FK_player_ratings_players_player_id",
                table: "player_ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_player_tournament_stats_players_player_id",
                table: "player_tournament_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_player_tournament_stats_tournaments_tournament_id",
                table: "player_tournament_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_rating_adjustments_matches_match_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_rating_adjustments_players_player_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_tournament_admin_notes_tournaments_ref_id",
                table: "tournament_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits");

            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_submitted_by_user_id",
                table: "tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_tournaments_users_verified_by_user_id",
                table: "tournaments");

            migrationBuilder.DropForeignKey(
                name: "FK_user_settings_users_user_id",
                table: "user_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_users_players_player_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_settings",
                table: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournaments",
                table: "tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournament_audits",
                table: "tournament_audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tournament_admin_notes",
                table: "tournament_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rating_adjustments",
                table: "rating_adjustments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_players",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_tournament_stats",
                table: "player_tournament_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_ratings",
                table: "player_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_osu_ruleset_data",
                table: "player_osu_ruleset_data");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_match_stats",
                table: "player_match_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_highest_ranks",
                table: "player_highest_ranks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_player_admin_notes",
                table: "player_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_oauth_clients",
                table: "oauth_clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_oauth_client_admin_notes",
                table: "oauth_client_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_matches",
                table: "matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_match_win_records",
                table: "match_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_match_audits",
                table: "match_audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_match_admin_notes",
                table: "match_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_games",
                table: "games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_win_records",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_scores",
                table: "game_scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_score_audits",
                table: "game_score_audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_score_admin_notes",
                table: "game_score_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_audits",
                table: "game_audits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_game_admin_notes",
                table: "game_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_beatmapsets",
                table: "beatmapsets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_beatmaps",
                table: "beatmaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_beatmap_attributes",
                table: "beatmap_attributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK___join__pooled_beatmaps",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK___join__beatmap_creators",
                table: "__join__beatmap_creators");

            migrationBuilder.RenameIndex(
                name: "IX_users_player_id",
                table: "users",
                newName: "ix_users_player_id");

            migrationBuilder.RenameColumn(
                name: "default_ruleset_controlled",
                table: "user_settings",
                newName: "default_ruleset_is_controlled");

            migrationBuilder.RenameIndex(
                name: "IX_user_settings_user_id",
                table: "user_settings",
                newName: "ix_user_settings_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_verified_by_user_id",
                table: "tournaments",
                newName: "ix_tournaments_verified_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_submitted_by_user_id",
                table: "tournaments",
                newName: "ix_tournaments_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_ruleset",
                table: "tournaments",
                newName: "ix_tournaments_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_tournaments_name_abbreviation",
                table: "tournaments",
                newName: "ix_tournaments_name_abbreviation");

            migrationBuilder.RenameColumn(
                name: "ref_id_lock",
                table: "tournament_audits",
                newName: "reference_id_lock");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "tournament_audits",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournament_audits_ref_id",
                table: "tournament_audits",
                newName: "ix_tournament_audits_reference_id");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "tournament_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournament_admin_notes_admin_user_id",
                table: "tournament_admin_notes",
                newName: "ix_tournament_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tournament_admin_notes_ref_id",
                table: "tournament_admin_notes",
                newName: "ix_tournament_admin_notes_reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_player_rating_id",
                table: "rating_adjustments",
                newName: "ix_rating_adjustments_player_rating_id");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_player_id_timestamp",
                table: "rating_adjustments",
                newName: "ix_rating_adjustments_player_id_timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_player_id_match_id",
                table: "rating_adjustments",
                newName: "ix_rating_adjustments_player_id_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_rating_adjustments_match_id",
                table: "rating_adjustments",
                newName: "ix_rating_adjustments_match_id");

            migrationBuilder.RenameColumn(
                name: "default_ruleset",
                table: "players",
                newName: "ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_players_osu_id",
                table: "players",
                newName: "ix_players_osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_tournament_stats_tournament_id",
                table: "player_tournament_stats",
                newName: "ix_player_tournament_stats_tournament_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_tournament_stats_player_id_tournament_id",
                table: "player_tournament_stats",
                newName: "ix_player_tournament_stats_player_id_tournament_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_ratings_ruleset",
                table: "player_ratings",
                newName: "ix_player_ratings_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_ratings_rating",
                table: "player_ratings",
                newName: "ix_player_ratings_rating");

            migrationBuilder.RenameIndex(
                name: "IX_player_ratings_player_id_ruleset",
                table: "player_ratings",
                newName: "ix_player_ratings_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_ratings_player_id",
                table: "player_ratings",
                newName: "ix_player_ratings_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_osu_ruleset_data_player_id_ruleset",
                table: "player_osu_ruleset_data",
                newName: "ix_player_osu_ruleset_data_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_match_stats_player_id_won",
                table: "player_match_stats",
                newName: "ix_player_match_stats_player_id_won");

            migrationBuilder.RenameIndex(
                name: "IX_player_match_stats_player_id_match_id",
                table: "player_match_stats",
                newName: "ix_player_match_stats_player_id_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_match_stats_player_id",
                table: "player_match_stats",
                newName: "ix_player_match_stats_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_match_stats_match_id",
                table: "player_match_stats",
                newName: "ix_player_match_stats_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_highest_ranks_player_id_ruleset",
                table: "player_highest_ranks",
                newName: "ix_player_highest_ranks_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "IX_player_highest_ranks_global_rank",
                table: "player_highest_ranks",
                newName: "ix_player_highest_ranks_global_rank");

            migrationBuilder.RenameIndex(
                name: "IX_player_highest_ranks_country_rank",
                table: "player_highest_ranks",
                newName: "ix_player_highest_ranks_country_rank");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "player_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_admin_notes_admin_user_id",
                table: "player_admin_notes",
                newName: "ix_player_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_player_admin_notes_ref_id",
                table: "player_admin_notes",
                newName: "ix_player_admin_notes_reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_oauth_clients_user_id",
                table: "oauth_clients",
                newName: "ix_oauth_clients_user_id");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "oauth_client_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_oauth_client_admin_notes_ref_id",
                table: "oauth_client_admin_notes",
                newName: "ix_oauth_client_admin_notes_reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_verified_by_user_id",
                table: "matches",
                newName: "ix_matches_verified_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_tournament_id",
                table: "matches",
                newName: "ix_matches_tournament_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_submitted_by_user_id",
                table: "matches",
                newName: "ix_matches_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_matches_osu_id",
                table: "matches",
                newName: "ix_matches_osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_winner_roster",
                table: "match_win_records",
                newName: "ix_match_win_records_winner_roster");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_match_id",
                table: "match_win_records",
                newName: "ix_match_win_records_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_win_records_loser_roster",
                table: "match_win_records",
                newName: "ix_match_win_records_loser_roster");

            migrationBuilder.RenameColumn(
                name: "ref_id_lock",
                table: "match_audits",
                newName: "reference_id_lock");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "match_audits",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_audits_ref_id",
                table: "match_audits",
                newName: "ix_match_audits_reference_id");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "match_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_admin_notes_admin_user_id",
                table: "match_admin_notes",
                newName: "ix_match_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_match_admin_notes_ref_id",
                table: "match_admin_notes",
                newName: "ix_match_admin_notes_reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_games_start_time",
                table: "games",
                newName: "ix_games_start_time");

            migrationBuilder.RenameIndex(
                name: "IX_games_osu_id",
                table: "games",
                newName: "ix_games_osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_games_match_id",
                table: "games",
                newName: "ix_games_match_id");

            migrationBuilder.RenameIndex(
                name: "IX_games_beatmap_id",
                table: "games",
                newName: "ix_games_beatmap_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_win_records_winner_roster",
                table: "game_win_records",
                newName: "ix_game_win_records_winner_roster");

            migrationBuilder.RenameIndex(
                name: "IX_game_win_records_game_id",
                table: "game_win_records",
                newName: "ix_game_win_records_game_id");

            migrationBuilder.RenameColumn(
                name: "count_50",
                table: "game_scores",
                newName: "count50");

            migrationBuilder.RenameColumn(
                name: "count_300",
                table: "game_scores",
                newName: "count300");

            migrationBuilder.RenameColumn(
                name: "count_100",
                table: "game_scores",
                newName: "count100");

            migrationBuilder.RenameIndex(
                name: "IX_game_scores_player_id_game_id",
                table: "game_scores",
                newName: "ix_game_scores_player_id_game_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_scores_player_id",
                table: "game_scores",
                newName: "ix_game_scores_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_scores_game_id",
                table: "game_scores",
                newName: "ix_game_scores_game_id");

            migrationBuilder.RenameColumn(
                name: "ref_id_lock",
                table: "game_score_audits",
                newName: "reference_id_lock");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "game_score_audits",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_score_audits_ref_id",
                table: "game_score_audits",
                newName: "ix_game_score_audits_reference_id");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "game_score_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_score_admin_notes_admin_user_id",
                table: "game_score_admin_notes",
                newName: "ix_game_score_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_score_admin_notes_ref_id",
                table: "game_score_admin_notes",
                newName: "ix_game_score_admin_notes_reference_id");

            migrationBuilder.RenameColumn(
                name: "ref_id_lock",
                table: "game_audits",
                newName: "reference_id_lock");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "game_audits",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_audits_ref_id",
                table: "game_audits",
                newName: "ix_game_audits_reference_id");

            migrationBuilder.RenameColumn(
                name: "ref_id",
                table: "game_admin_notes",
                newName: "reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_admin_notes_admin_user_id",
                table: "game_admin_notes",
                newName: "ix_game_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_game_admin_notes_ref_id",
                table: "game_admin_notes",
                newName: "ix_game_admin_notes_reference_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmapsets_osu_id",
                table: "beatmapsets",
                newName: "ix_beatmapsets_osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmapsets_creator_id",
                table: "beatmapsets",
                newName: "ix_beatmapsets_creator_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmaps_osu_id",
                table: "beatmaps",
                newName: "ix_beatmaps_osu_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmaps_beatmapset_id",
                table: "beatmaps",
                newName: "ix_beatmaps_beatmapset_id");

            migrationBuilder.RenameIndex(
                name: "IX_beatmap_attributes_beatmap_id_mods",
                table: "beatmap_attributes",
                newName: "ix_beatmap_attributes_beatmap_id_mods");

            migrationBuilder.RenameColumn(
                name: "TournamentsPooledInId",
                table: "__join__pooled_beatmaps",
                newName: "tournaments_pooled_in_id");

            migrationBuilder.RenameColumn(
                name: "PooledBeatmapsId",
                table: "__join__pooled_beatmaps",
                newName: "pooled_beatmaps_id");

            migrationBuilder.RenameIndex(
                name: "IX___join__pooled_beatmaps_TournamentsPooledInId",
                table: "__join__pooled_beatmaps",
                newName: "ix___join__pooled_beatmaps_tournaments_pooled_in_id");

            migrationBuilder.RenameColumn(
                name: "CreatorsId",
                table: "__join__beatmap_creators",
                newName: "creators_id");

            migrationBuilder.RenameColumn(
                name: "CreatedBeatmapsId",
                table: "__join__beatmap_creators",
                newName: "created_beatmaps_id");

            migrationBuilder.RenameIndex(
                name: "IX___join__beatmap_creators_CreatorsId",
                table: "__join__beatmap_creators",
                newName: "ix___join__beatmap_creators_creators_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_settings",
                table: "user_settings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tournaments",
                table: "tournaments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tournament_audits",
                table: "tournament_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tournament_admin_notes",
                table: "tournament_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rating_adjustments",
                table: "rating_adjustments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_players",
                table: "players",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_tournament_stats",
                table: "player_tournament_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_ratings",
                table: "player_ratings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_osu_ruleset_data",
                table: "player_osu_ruleset_data",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_match_stats",
                table: "player_match_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_highest_ranks",
                table: "player_highest_ranks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_player_admin_notes",
                table: "player_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_oauth_clients",
                table: "oauth_clients",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_oauth_client_admin_notes",
                table: "oauth_client_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_matches",
                table: "matches",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_match_win_records",
                table: "match_win_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_match_audits",
                table: "match_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_match_admin_notes",
                table: "match_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_games",
                table: "games",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_win_records",
                table: "game_win_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_scores",
                table: "game_scores",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_score_audits",
                table: "game_score_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_score_admin_notes",
                table: "game_score_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_audits",
                table: "game_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_admin_notes",
                table: "game_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_beatmapsets",
                table: "beatmapsets",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_beatmaps",
                table: "beatmaps",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_beatmap_attributes",
                table: "beatmap_attributes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk___join__pooled_beatmaps",
                table: "__join__pooled_beatmaps",
                columns: ["pooled_beatmaps_id", "tournaments_pooled_in_id"]);

            migrationBuilder.AddPrimaryKey(
                name: "pk___join__beatmap_creators",
                table: "__join__beatmap_creators",
                columns: ["created_beatmaps_id", "creators_id"]);

            migrationBuilder.AddForeignKey(
                name: "fk___join__beatmap_creators_beatmaps_created_beatmaps_id",
                table: "__join__beatmap_creators",
                column: "created_beatmaps_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk___join__beatmap_creators_players_creators_id",
                table: "__join__beatmap_creators",
                column: "creators_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk___join__pooled_beatmaps_beatmaps_pooled_beatmaps_id",
                table: "__join__pooled_beatmaps",
                column: "pooled_beatmaps_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk___join__pooled_beatmaps_tournaments_tournaments_pooled_in_id",
                table: "__join__pooled_beatmaps",
                column: "tournaments_pooled_in_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_beatmap_attributes_beatmaps_beatmap_id",
                table: "beatmap_attributes",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_beatmaps_beatmapsets_beatmapset_id",
                table: "beatmaps",
                column: "beatmapset_id",
                principalTable: "beatmapsets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_beatmapsets_players_creator_id",
                table: "beatmapsets",
                column: "creator_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_admin_notes_games_reference_id",
                table: "game_admin_notes",
                column: "reference_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_admin_notes_users_admin_user_id",
                table: "game_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_audits_games_reference_id",
                table: "game_audits",
                column: "reference_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_game_score_admin_notes_game_scores_reference_id",
                table: "game_score_admin_notes",
                column: "reference_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_score_audits_game_scores_reference_id",
                table: "game_score_audits",
                column: "reference_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_game_scores_games_game_id",
                table: "game_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_scores_players_player_id",
                table: "game_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_win_records_games_game_id",
                table: "game_win_records",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_games_matches_match_id",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_admin_notes_matches_reference_id",
                table: "match_admin_notes",
                column: "reference_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_admin_notes_users_admin_user_id",
                table: "match_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_audits_matches_reference_id",
                table: "match_audits",
                column: "reference_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_match_win_records_matches_match_id",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_matches_tournaments_tournament_id",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_matches_users_submitted_by_user_id",
                table: "matches",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_matches_users_verified_by_user_id",
                table: "matches",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_oauth_client_admin_notes_oauth_clients_reference_id",
                table: "oauth_client_admin_notes",
                column: "reference_id",
                principalTable: "oauth_clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_oauth_clients_users_user_id",
                table: "oauth_clients",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_admin_notes_players_reference_id",
                table: "player_admin_notes",
                column: "reference_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_admin_notes_users_admin_user_id",
                table: "player_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_highest_ranks_players_player_id",
                table: "player_highest_ranks",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_match_stats_matches_match_id",
                table: "player_match_stats",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_match_stats_players_player_id",
                table: "player_match_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_osu_ruleset_data_players_player_id",
                table: "player_osu_ruleset_data",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_ratings_players_player_id",
                table: "player_ratings",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_tournament_stats_players_player_id",
                table: "player_tournament_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_player_tournament_stats_tournaments_tournament_id",
                table: "player_tournament_stats",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rating_adjustments_matches_match_id",
                table: "rating_adjustments",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments",
                column: "player_rating_id",
                principalTable: "player_ratings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rating_adjustments_players_player_id",
                table: "rating_adjustments",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tournament_admin_notes_tournaments_reference_id",
                table: "tournament_admin_notes",
                column: "reference_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tournament_audits_tournaments_reference_id",
                table: "tournament_audits",
                column: "reference_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tournaments_users_submitted_by_user_id",
                table: "tournaments",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tournaments_users_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_user_settings_users_user_id",
                table: "user_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_players_player_id",
                table: "users",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk___join__beatmap_creators_beatmaps_created_beatmaps_id",
                table: "__join__beatmap_creators");

            migrationBuilder.DropForeignKey(
                name: "fk___join__beatmap_creators_players_creators_id",
                table: "__join__beatmap_creators");

            migrationBuilder.DropForeignKey(
                name: "fk___join__pooled_beatmaps_beatmaps_pooled_beatmaps_id",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropForeignKey(
                name: "fk___join__pooled_beatmaps_tournaments_tournaments_pooled_in_id",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropForeignKey(
                name: "fk_beatmap_attributes_beatmaps_beatmap_id",
                table: "beatmap_attributes");

            migrationBuilder.DropForeignKey(
                name: "fk_beatmaps_beatmapsets_beatmapset_id",
                table: "beatmaps");

            migrationBuilder.DropForeignKey(
                name: "fk_beatmapsets_players_creator_id",
                table: "beatmapsets");

            migrationBuilder.DropForeignKey(
                name: "fk_game_admin_notes_games_reference_id",
                table: "game_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_game_admin_notes_users_admin_user_id",
                table: "game_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_game_audits_games_reference_id",
                table: "game_audits");

            migrationBuilder.DropForeignKey(
                name: "fk_game_score_admin_notes_game_scores_reference_id",
                table: "game_score_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_game_score_audits_game_scores_reference_id",
                table: "game_score_audits");

            migrationBuilder.DropForeignKey(
                name: "fk_game_scores_games_game_id",
                table: "game_scores");

            migrationBuilder.DropForeignKey(
                name: "fk_game_scores_players_player_id",
                table: "game_scores");

            migrationBuilder.DropForeignKey(
                name: "fk_game_win_records_games_game_id",
                table: "game_win_records");

            migrationBuilder.DropForeignKey(
                name: "fk_games_beatmaps_beatmap_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "fk_games_matches_match_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "fk_match_admin_notes_matches_reference_id",
                table: "match_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_match_admin_notes_users_admin_user_id",
                table: "match_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_match_audits_matches_reference_id",
                table: "match_audits");

            migrationBuilder.DropForeignKey(
                name: "fk_match_win_records_matches_match_id",
                table: "match_win_records");

            migrationBuilder.DropForeignKey(
                name: "fk_matches_tournaments_tournament_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "fk_matches_users_submitted_by_user_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "fk_matches_users_verified_by_user_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "fk_oauth_client_admin_notes_oauth_clients_reference_id",
                table: "oauth_client_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_oauth_clients_users_user_id",
                table: "oauth_clients");

            migrationBuilder.DropForeignKey(
                name: "fk_player_admin_notes_players_reference_id",
                table: "player_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_player_admin_notes_users_admin_user_id",
                table: "player_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_player_highest_ranks_players_player_id",
                table: "player_highest_ranks");

            migrationBuilder.DropForeignKey(
                name: "fk_player_match_stats_matches_match_id",
                table: "player_match_stats");

            migrationBuilder.DropForeignKey(
                name: "fk_player_match_stats_players_player_id",
                table: "player_match_stats");

            migrationBuilder.DropForeignKey(
                name: "fk_player_osu_ruleset_data_players_player_id",
                table: "player_osu_ruleset_data");

            migrationBuilder.DropForeignKey(
                name: "fk_player_ratings_players_player_id",
                table: "player_ratings");

            migrationBuilder.DropForeignKey(
                name: "fk_player_tournament_stats_players_player_id",
                table: "player_tournament_stats");

            migrationBuilder.DropForeignKey(
                name: "fk_player_tournament_stats_tournaments_tournament_id",
                table: "player_tournament_stats");

            migrationBuilder.DropForeignKey(
                name: "fk_rating_adjustments_matches_match_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_rating_adjustments_players_player_id",
                table: "rating_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_tournament_admin_notes_tournaments_reference_id",
                table: "tournament_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes");

            migrationBuilder.DropForeignKey(
                name: "fk_tournament_audits_tournaments_reference_id",
                table: "tournament_audits");

            migrationBuilder.DropForeignKey(
                name: "fk_tournaments_users_submitted_by_user_id",
                table: "tournaments");

            migrationBuilder.DropForeignKey(
                name: "fk_tournaments_users_verified_by_user_id",
                table: "tournaments");

            migrationBuilder.DropForeignKey(
                name: "fk_user_settings_users_user_id",
                table: "user_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_users_players_player_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_settings",
                table: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tournaments",
                table: "tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tournament_audits",
                table: "tournament_audits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tournament_admin_notes",
                table: "tournament_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rating_adjustments",
                table: "rating_adjustments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_players",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_tournament_stats",
                table: "player_tournament_stats");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_ratings",
                table: "player_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_osu_ruleset_data",
                table: "player_osu_ruleset_data");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_match_stats",
                table: "player_match_stats");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_highest_ranks",
                table: "player_highest_ranks");

            migrationBuilder.DropPrimaryKey(
                name: "pk_player_admin_notes",
                table: "player_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_oauth_clients",
                table: "oauth_clients");

            migrationBuilder.DropPrimaryKey(
                name: "pk_oauth_client_admin_notes",
                table: "oauth_client_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_matches",
                table: "matches");

            migrationBuilder.DropPrimaryKey(
                name: "pk_match_win_records",
                table: "match_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "pk_match_audits",
                table: "match_audits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_match_admin_notes",
                table: "match_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_games",
                table: "games");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_win_records",
                table: "game_win_records");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_scores",
                table: "game_scores");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_score_audits",
                table: "game_score_audits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_score_admin_notes",
                table: "game_score_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_audits",
                table: "game_audits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_game_admin_notes",
                table: "game_admin_notes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_beatmapsets",
                table: "beatmapsets");

            migrationBuilder.DropPrimaryKey(
                name: "pk_beatmaps",
                table: "beatmaps");

            migrationBuilder.DropPrimaryKey(
                name: "pk_beatmap_attributes",
                table: "beatmap_attributes");

            migrationBuilder.DropPrimaryKey(
                name: "pk___join__pooled_beatmaps",
                table: "__join__pooled_beatmaps");

            migrationBuilder.DropPrimaryKey(
                name: "pk___join__beatmap_creators",
                table: "__join__beatmap_creators");

            migrationBuilder.RenameIndex(
                name: "ix_users_player_id",
                table: "users",
                newName: "IX_users_player_id");

            migrationBuilder.RenameColumn(
                name: "default_ruleset_is_controlled",
                table: "user_settings",
                newName: "default_ruleset_controlled");

            migrationBuilder.RenameIndex(
                name: "ix_user_settings_user_id",
                table: "user_settings",
                newName: "IX_user_settings_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournaments_verified_by_user_id",
                table: "tournaments",
                newName: "IX_tournaments_verified_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournaments_submitted_by_user_id",
                table: "tournaments",
                newName: "IX_tournaments_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournaments_ruleset",
                table: "tournaments",
                newName: "IX_tournaments_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_tournaments_name_abbreviation",
                table: "tournaments",
                newName: "IX_tournaments_name_abbreviation");

            migrationBuilder.RenameColumn(
                name: "reference_id_lock",
                table: "tournament_audits",
                newName: "ref_id_lock");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "tournament_audits",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournament_audits_reference_id",
                table: "tournament_audits",
                newName: "IX_tournament_audits_ref_id");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "tournament_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournament_admin_notes_admin_user_id",
                table: "tournament_admin_notes",
                newName: "IX_tournament_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_tournament_admin_notes_reference_id",
                table: "tournament_admin_notes",
                newName: "IX_tournament_admin_notes_ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_rating_adjustments_player_rating_id",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_player_rating_id");

            migrationBuilder.RenameIndex(
                name: "ix_rating_adjustments_player_id_timestamp",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_player_id_timestamp");

            migrationBuilder.RenameIndex(
                name: "ix_rating_adjustments_player_id_match_id",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_player_id_match_id");

            migrationBuilder.RenameIndex(
                name: "ix_rating_adjustments_match_id",
                table: "rating_adjustments",
                newName: "IX_rating_adjustments_match_id");

            migrationBuilder.RenameColumn(
                name: "ruleset",
                table: "players",
                newName: "default_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_players_osu_id",
                table: "players",
                newName: "IX_players_osu_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_tournament_stats_tournament_id",
                table: "player_tournament_stats",
                newName: "IX_player_tournament_stats_tournament_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_tournament_stats_player_id_tournament_id",
                table: "player_tournament_stats",
                newName: "IX_player_tournament_stats_player_id_tournament_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_ratings_ruleset",
                table: "player_ratings",
                newName: "IX_player_ratings_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_player_ratings_rating",
                table: "player_ratings",
                newName: "IX_player_ratings_rating");

            migrationBuilder.RenameIndex(
                name: "ix_player_ratings_player_id_ruleset",
                table: "player_ratings",
                newName: "IX_player_ratings_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_player_ratings_player_id",
                table: "player_ratings",
                newName: "IX_player_ratings_player_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_osu_ruleset_data_player_id_ruleset",
                table: "player_osu_ruleset_data",
                newName: "IX_player_osu_ruleset_data_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_player_match_stats_player_id_won",
                table: "player_match_stats",
                newName: "IX_player_match_stats_player_id_won");

            migrationBuilder.RenameIndex(
                name: "ix_player_match_stats_player_id_match_id",
                table: "player_match_stats",
                newName: "IX_player_match_stats_player_id_match_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_match_stats_player_id",
                table: "player_match_stats",
                newName: "IX_player_match_stats_player_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_match_stats_match_id",
                table: "player_match_stats",
                newName: "IX_player_match_stats_match_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_highest_ranks_player_id_ruleset",
                table: "player_highest_ranks",
                newName: "IX_player_highest_ranks_player_id_ruleset");

            migrationBuilder.RenameIndex(
                name: "ix_player_highest_ranks_global_rank",
                table: "player_highest_ranks",
                newName: "IX_player_highest_ranks_global_rank");

            migrationBuilder.RenameIndex(
                name: "ix_player_highest_ranks_country_rank",
                table: "player_highest_ranks",
                newName: "IX_player_highest_ranks_country_rank");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "player_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_admin_notes_admin_user_id",
                table: "player_admin_notes",
                newName: "IX_player_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_player_admin_notes_reference_id",
                table: "player_admin_notes",
                newName: "IX_player_admin_notes_ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_oauth_clients_user_id",
                table: "oauth_clients",
                newName: "IX_oauth_clients_user_id");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "oauth_client_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_oauth_client_admin_notes_reference_id",
                table: "oauth_client_admin_notes",
                newName: "IX_oauth_client_admin_notes_ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_matches_verified_by_user_id",
                table: "matches",
                newName: "IX_matches_verified_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_matches_tournament_id",
                table: "matches",
                newName: "IX_matches_tournament_id");

            migrationBuilder.RenameIndex(
                name: "ix_matches_submitted_by_user_id",
                table: "matches",
                newName: "IX_matches_submitted_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_matches_osu_id",
                table: "matches",
                newName: "IX_matches_osu_id");

            migrationBuilder.RenameIndex(
                name: "ix_match_win_records_winner_roster",
                table: "match_win_records",
                newName: "IX_match_win_records_winner_roster");

            migrationBuilder.RenameIndex(
                name: "ix_match_win_records_match_id",
                table: "match_win_records",
                newName: "IX_match_win_records_match_id");

            migrationBuilder.RenameIndex(
                name: "ix_match_win_records_loser_roster",
                table: "match_win_records",
                newName: "IX_match_win_records_loser_roster");

            migrationBuilder.RenameColumn(
                name: "reference_id_lock",
                table: "match_audits",
                newName: "ref_id_lock");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "match_audits",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_match_audits_reference_id",
                table: "match_audits",
                newName: "IX_match_audits_ref_id");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "match_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_match_admin_notes_admin_user_id",
                table: "match_admin_notes",
                newName: "IX_match_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_match_admin_notes_reference_id",
                table: "match_admin_notes",
                newName: "IX_match_admin_notes_ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_games_start_time",
                table: "games",
                newName: "IX_games_start_time");

            migrationBuilder.RenameIndex(
                name: "ix_games_osu_id",
                table: "games",
                newName: "IX_games_osu_id");

            migrationBuilder.RenameIndex(
                name: "ix_games_match_id",
                table: "games",
                newName: "IX_games_match_id");

            migrationBuilder.RenameIndex(
                name: "ix_games_beatmap_id",
                table: "games",
                newName: "IX_games_beatmap_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_win_records_winner_roster",
                table: "game_win_records",
                newName: "IX_game_win_records_winner_roster");

            migrationBuilder.RenameIndex(
                name: "ix_game_win_records_game_id",
                table: "game_win_records",
                newName: "IX_game_win_records_game_id");

            migrationBuilder.RenameColumn(
                name: "count50",
                table: "game_scores",
                newName: "count_50");

            migrationBuilder.RenameColumn(
                name: "count300",
                table: "game_scores",
                newName: "count_300");

            migrationBuilder.RenameColumn(
                name: "count100",
                table: "game_scores",
                newName: "count_100");

            migrationBuilder.RenameIndex(
                name: "ix_game_scores_player_id_game_id",
                table: "game_scores",
                newName: "IX_game_scores_player_id_game_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_scores_player_id",
                table: "game_scores",
                newName: "IX_game_scores_player_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_scores_game_id",
                table: "game_scores",
                newName: "IX_game_scores_game_id");

            migrationBuilder.RenameColumn(
                name: "reference_id_lock",
                table: "game_score_audits",
                newName: "ref_id_lock");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "game_score_audits",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_score_audits_reference_id",
                table: "game_score_audits",
                newName: "IX_game_score_audits_ref_id");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "game_score_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_score_admin_notes_admin_user_id",
                table: "game_score_admin_notes",
                newName: "IX_game_score_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_score_admin_notes_reference_id",
                table: "game_score_admin_notes",
                newName: "IX_game_score_admin_notes_ref_id");

            migrationBuilder.RenameColumn(
                name: "reference_id_lock",
                table: "game_audits",
                newName: "ref_id_lock");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "game_audits",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_audits_reference_id",
                table: "game_audits",
                newName: "IX_game_audits_ref_id");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "game_admin_notes",
                newName: "ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_admin_notes_admin_user_id",
                table: "game_admin_notes",
                newName: "IX_game_admin_notes_admin_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_game_admin_notes_reference_id",
                table: "game_admin_notes",
                newName: "IX_game_admin_notes_ref_id");

            migrationBuilder.RenameIndex(
                name: "ix_beatmapsets_osu_id",
                table: "beatmapsets",
                newName: "IX_beatmapsets_osu_id");

            migrationBuilder.RenameIndex(
                name: "ix_beatmapsets_creator_id",
                table: "beatmapsets",
                newName: "IX_beatmapsets_creator_id");

            migrationBuilder.RenameIndex(
                name: "ix_beatmaps_osu_id",
                table: "beatmaps",
                newName: "IX_beatmaps_osu_id");

            migrationBuilder.RenameIndex(
                name: "ix_beatmaps_beatmapset_id",
                table: "beatmaps",
                newName: "IX_beatmaps_beatmapset_id");

            migrationBuilder.RenameIndex(
                name: "ix_beatmap_attributes_beatmap_id_mods",
                table: "beatmap_attributes",
                newName: "IX_beatmap_attributes_beatmap_id_mods");

            migrationBuilder.RenameColumn(
                name: "tournaments_pooled_in_id",
                table: "__join__pooled_beatmaps",
                newName: "TournamentsPooledInId");

            migrationBuilder.RenameColumn(
                name: "pooled_beatmaps_id",
                table: "__join__pooled_beatmaps",
                newName: "PooledBeatmapsId");

            migrationBuilder.RenameIndex(
                name: "ix___join__pooled_beatmaps_tournaments_pooled_in_id",
                table: "__join__pooled_beatmaps",
                newName: "IX___join__pooled_beatmaps_TournamentsPooledInId");

            migrationBuilder.RenameColumn(
                name: "creators_id",
                table: "__join__beatmap_creators",
                newName: "CreatorsId");

            migrationBuilder.RenameColumn(
                name: "created_beatmaps_id",
                table: "__join__beatmap_creators",
                newName: "CreatedBeatmapsId");

            migrationBuilder.RenameIndex(
                name: "ix___join__beatmap_creators_creators_id",
                table: "__join__beatmap_creators",
                newName: "IX___join__beatmap_creators_CreatorsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_settings",
                table: "user_settings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournaments",
                table: "tournaments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournament_audits",
                table: "tournament_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tournament_admin_notes",
                table: "tournament_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rating_adjustments",
                table: "rating_adjustments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_players",
                table: "players",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_tournament_stats",
                table: "player_tournament_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_ratings",
                table: "player_ratings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_osu_ruleset_data",
                table: "player_osu_ruleset_data",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_match_stats",
                table: "player_match_stats",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_highest_ranks",
                table: "player_highest_ranks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player_admin_notes",
                table: "player_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_oauth_clients",
                table: "oauth_clients",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_oauth_client_admin_notes",
                table: "oauth_client_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_matches",
                table: "matches",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_win_records",
                table: "match_win_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_audits",
                table: "match_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_match_admin_notes",
                table: "match_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_games",
                table: "games",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_win_records",
                table: "game_win_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_scores",
                table: "game_scores",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_score_audits",
                table: "game_score_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_score_admin_notes",
                table: "game_score_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_audits",
                table: "game_audits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_game_admin_notes",
                table: "game_admin_notes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_beatmapsets",
                table: "beatmapsets",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_beatmaps",
                table: "beatmaps",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_beatmap_attributes",
                table: "beatmap_attributes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK___join__pooled_beatmaps",
                table: "__join__pooled_beatmaps",
                columns: ["PooledBeatmapsId", "TournamentsPooledInId"]);

            migrationBuilder.AddPrimaryKey(
                name: "PK___join__beatmap_creators",
                table: "__join__beatmap_creators",
                columns: ["CreatedBeatmapsId", "CreatorsId"]);

            migrationBuilder.AddForeignKey(
                name: "FK___join__beatmap_creators_beatmaps_CreatedBeatmapsId",
                table: "__join__beatmap_creators",
                column: "CreatedBeatmapsId",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK___join__beatmap_creators_players_CreatorsId",
                table: "__join__beatmap_creators",
                column: "CreatorsId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK___join__pooled_beatmaps_beatmaps_PooledBeatmapsId",
                table: "__join__pooled_beatmaps",
                column: "PooledBeatmapsId",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK___join__pooled_beatmaps_tournaments_TournamentsPooledInId",
                table: "__join__pooled_beatmaps",
                column: "TournamentsPooledInId",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beatmap_attributes_beatmaps_beatmap_id",
                table: "beatmap_attributes",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beatmaps_beatmapsets_beatmapset_id",
                table: "beatmaps",
                column: "beatmapset_id",
                principalTable: "beatmapsets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beatmapsets_players_creator_id",
                table: "beatmapsets",
                column: "creator_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_admin_notes_games_ref_id",
                table: "game_admin_notes",
                column: "ref_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_admin_notes_users_admin_user_id",
                table: "game_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_audits_games_ref_id",
                table: "game_audits",
                column: "ref_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_admin_notes_game_scores_ref_id",
                table: "game_score_admin_notes",
                column: "ref_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_admin_notes_users_admin_user_id",
                table: "game_score_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_score_audits_game_scores_ref_id",
                table: "game_score_audits",
                column: "ref_id",
                principalTable: "game_scores",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_game_scores_games_game_id",
                table: "game_scores",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_scores_players_player_id",
                table: "game_scores",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_game_win_records_games_game_id",
                table: "game_win_records",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_games_beatmaps_beatmap_id",
                table: "games",
                column: "beatmap_id",
                principalTable: "beatmaps",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_games_matches_match_id",
                table: "games",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_match_admin_notes_matches_ref_id",
                table: "match_admin_notes",
                column: "ref_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_match_admin_notes_users_admin_user_id",
                table: "match_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_match_audits_matches_ref_id",
                table: "match_audits",
                column: "ref_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_match_win_records_matches_match_id",
                table: "match_win_records",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_tournaments_tournament_id",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_submitted_by_user_id",
                table: "matches",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_users_verified_by_user_id",
                table: "matches",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_oauth_client_admin_notes_oauth_clients_ref_id",
                table: "oauth_client_admin_notes",
                column: "ref_id",
                principalTable: "oauth_clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_oauth_clients_users_user_id",
                table: "oauth_clients",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_admin_notes_players_ref_id",
                table: "player_admin_notes",
                column: "ref_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_admin_notes_users_admin_user_id",
                table: "player_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_highest_ranks_players_player_id",
                table: "player_highest_ranks",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_match_stats_matches_match_id",
                table: "player_match_stats",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_match_stats_players_player_id",
                table: "player_match_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_osu_ruleset_data_players_player_id",
                table: "player_osu_ruleset_data",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_ratings_players_player_id",
                table: "player_ratings",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_tournament_stats_players_player_id",
                table: "player_tournament_stats",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_player_tournament_stats_tournaments_tournament_id",
                table: "player_tournament_stats",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rating_adjustments_matches_match_id",
                table: "rating_adjustments",
                column: "match_id",
                principalTable: "matches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rating_adjustments_player_ratings_player_rating_id",
                table: "rating_adjustments",
                column: "player_rating_id",
                principalTable: "player_ratings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rating_adjustments_players_player_id",
                table: "rating_adjustments",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_admin_notes_tournaments_ref_id",
                table: "tournament_admin_notes",
                column: "ref_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_admin_notes_users_admin_user_id",
                table: "tournament_admin_notes",
                column: "admin_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tournament_audits_tournaments_ref_id",
                table: "tournament_audits",
                column: "ref_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_submitted_by_user_id",
                table: "tournaments",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tournaments_users_verified_by_user_id",
                table: "tournaments",
                column: "verified_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_settings_users_user_id",
                table: "user_settings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_players_player_id",
                table: "users",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
