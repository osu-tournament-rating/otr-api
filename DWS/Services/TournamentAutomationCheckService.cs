using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;

namespace DWS.Services;

/// <summary>
/// Service for processing automation checks on tournaments.
/// Orchestrates all child entity checks (scores, games, matches) before running tournament-level checks.
/// </summary>
public class TournamentAutomationCheckService(
    ILogger<TournamentAutomationCheckService> logger,
    ITournamentsRepository tournamentsRepository,
    TournamentAutomationChecks tournamentAutomationChecks,
    MatchAutomationChecks matchAutomationChecks,
    GameAutomationChecks gameAutomationChecks,
    ScoreAutomationChecks scoreAutomationChecks) : ITournamentAutomationCheckService
{
    /// <inheritdoc />
    public async Task<bool> ProcessAutomationChecksAsync(int entityId, bool overrideVerifiedState = false)
    {
        logger.LogInformation("Processing automation checks for tournament {TournamentId}", entityId);

        // Fetch the tournament with eager loading of navigation properties
        Tournament? tournament = await tournamentsRepository.GetAsync(entityId, eagerLoad: true);
        if (tournament is null)
        {
            logger.LogWarning("Tournament {TournamentId} not found", entityId);
            return false;
        }

        // Check if the entity needs processing based on current verification status
        if (!overrideVerifiedState &&
            (tournament.VerificationStatus == VerificationStatus.Verified ||
             tournament.VerificationStatus == VerificationStatus.Rejected))
        {
            logger.LogInformation(
                "Skipping automation checks for tournament {TournamentId} with verification status {VerificationStatus}",
                entityId,
                tournament.VerificationStatus);
            return tournament.VerificationStatus == VerificationStatus.Verified;
        }

        // Reset verification status if overriding
        if (overrideVerifiedState)
        {
            tournament.VerificationStatus = VerificationStatus.None;
            logger.LogInformation(
                "Resetting verification status to None for tournament {TournamentId} due to override",
                entityId);
        }

        // Load matches with games and scores for automation checks
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        // Process all child entities first (bottom-up approach)
        ProcessAllScores(tournament);
        ProcessAllGames(tournament);
        ProcessAllMatches(tournament);

        // Run tournament-level automation checks
        TournamentRejectionReason rejectionReason = tournamentAutomationChecks.Process(tournament);

        // Update verification status based on the result
        if (rejectionReason == TournamentRejectionReason.None)
        {
            tournament.VerificationStatus = VerificationStatus.PreVerified;
            tournament.RejectionReason = TournamentRejectionReason.None;
            logger.LogInformation("Tournament {TournamentId} passed all automation checks", entityId);
        }
        else
        {
            tournament.VerificationStatus = VerificationStatus.PreRejected;
            tournament.RejectionReason = rejectionReason;
            logger.LogInformation("Tournament {TournamentId} rejected by automation checks: {RejectionReason}", entityId, rejectionReason);
        }

        // Save all changes in a single transaction
        await tournamentsRepository.UpdateAsync(tournament);

        return rejectionReason == TournamentRejectionReason.None;
    }

    /// <summary>
    /// Processes automation checks for all scores in the tournament
    /// </summary>
    private void ProcessAllScores(Tournament tournament)
    {
        logger.LogDebug("Processing automation checks for all scores in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            foreach (Game game in match.Games)
            {
                foreach (GameScore score in game.Scores)
                {
                    ScoreRejectionReason scoreRejectionReason = scoreAutomationChecks.Process(score, tournament.Ruleset);

                    // Update score verification status
                    if (scoreRejectionReason == ScoreRejectionReason.None)
                    {
                        score.VerificationStatus = VerificationStatus.PreVerified;
                        score.RejectionReason = ScoreRejectionReason.None;
                    }
                    else
                    {
                        score.VerificationStatus = VerificationStatus.PreRejected;
                        score.RejectionReason = scoreRejectionReason;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Processes automation checks for all games in the tournament
    /// </summary>
    private void ProcessAllGames(Tournament tournament)
    {
        logger.LogDebug("Processing automation checks for all games in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            foreach (Game game in match.Games)
            {
                GameRejectionReason gameRejectionReason = gameAutomationChecks.Process(game, tournament);

                // Update game verification status
                if (gameRejectionReason == GameRejectionReason.None)
                {
                    game.VerificationStatus = VerificationStatus.PreVerified;
                    game.RejectionReason = GameRejectionReason.None;
                }
                else
                {
                    game.VerificationStatus = VerificationStatus.PreRejected;
                    game.RejectionReason = gameRejectionReason;
                }
            }
        }
    }

    /// <summary>
    /// Processes automation checks for all matches in the tournament
    /// </summary>
    private void ProcessAllMatches(Tournament tournament)
    {
        logger.LogDebug("Processing automation checks for all matches in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            MatchRejectionReason matchRejectionReason = matchAutomationChecks.Process(match, tournament);

            // Update match verification status
            if (matchRejectionReason == MatchRejectionReason.None)
            {
                match.VerificationStatus = VerificationStatus.PreVerified;
                match.RejectionReason = MatchRejectionReason.None;
            }
            else
            {
                match.VerificationStatus = VerificationStatus.PreRejected;
                match.RejectionReason = matchRejectionReason;
            }
        }
    }
}
