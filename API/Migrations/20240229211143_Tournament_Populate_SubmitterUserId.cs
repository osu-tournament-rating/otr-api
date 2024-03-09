using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Tournament_Populate_SubmitterUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE tournaments
                SET submitter_id = first_match.submitted_by_user
                FROM (
                    SELECT
                        MIN(created) as first_match_created,
                        submitted_by_user,
                        tournament_id
                    FROM matches
                    GROUP BY tournament_id, submitted_by_user
                ) AS first_match
                WHERE tournaments.id = first_match.tournament_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE matches
                SET submitted_by_user = tournaments.submitter_id
                FROM tournaments
                WHERE matches.tournament_id = tournaments.id
            ");
        }
    }
}