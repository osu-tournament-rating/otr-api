using Common.Enums.Verification;
using Database.Entities;
using Database.Repositories.Interfaces;
using DWS.AutomationChecks;
using DWS.Models;
using DWS.Services.Interfaces;

namespace DWS.Services.Implementations;

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

        // Store the tournament status before processing
        VerificationStatus tournamentStatusBefore = tournament.VerificationStatus;

        // Load matches with games and scores for automation checks
        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        // Perform HeadToHead to TeamVs conversion unless tournament is already verified
        // Rejected tournaments may have been rejected on submission but still need conversion
        if (tournament.VerificationStatus != VerificationStatus.Verified)
        {
            PerformHeadToHeadConversion(tournament);
        }

        // Check if automation checks should be skipped based on current verification status
        bool skipAutomationChecks = !overrideVerifiedState &&
            tournament.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected;

        if (skipAutomationChecks)
        {
            logger.LogInformation(
                "Skipping automation checks for tournament {TournamentId} with verification status {VerificationStatus}",
                entityId,
                tournament.VerificationStatus);

            // Save any changes from HeadToHead conversion (if it was performed for a Rejected tournament)
            if (tournament.VerificationStatus == VerificationStatus.Rejected)
            {
                await tournamentsRepository.UpdateAsync(tournament);
            }

            return tournament.VerificationStatus == VerificationStatus.Verified;
        }

        // Reset verification status and rejection reason if overriding
        if (overrideVerifiedState)
        {
            tournament.VerificationStatus = VerificationStatus.None;
            tournament.RejectionReason = TournamentRejectionReason.None;
            logger.LogInformation(
                "Resetting verification status to None for tournament {TournamentId} due to override",
                entityId);
        }

        // Process all child entities first (bottom-up approach)
        ProcessAllScores(tournament, overrideVerifiedState);
        ProcessAllGames(tournament, overrideVerifiedState);
        ProcessAllMatches(tournament, overrideVerifiedState);

        // Run tournament-level automation checks
        TournamentRejectionReason rejectionReason = tournamentAutomationChecks.Process(tournament);

        // Update verification status based on the result
        if (rejectionReason == TournamentRejectionReason.None)
        {
            tournament.VerificationStatus = VerificationStatus.PreVerified;
            tournament.RejectionReason = TournamentRejectionReason.None;
        }
        else
        {
            tournament.VerificationStatus = VerificationStatus.PreRejected;
            tournament.RejectionReason = rejectionReason;
        }

        // Capture detailed state after processing
        TournamentProcessingState processingState = TournamentProcessingReporter.CaptureDetailedState(tournament, tournamentStatusBefore);

        // Save all changes in a single transaction
        await tournamentsRepository.UpdateAsync(tournament);

        // Generate and log the detailed multi-line report
        string detailedReport = TournamentProcessingReporter.GenerateDetailedReport(processingState);
        logger.LogInformation("{DetailedReport}", detailedReport);

        return rejectionReason == TournamentRejectionReason.None;
    }

    /// <summary>
    /// Processes automation checks for all scores in the tournament
    /// </summary>
    /// <param name="tournament">The tournament containing the scores to process</param>
    /// <param name="overrideVerifiedState">Whether to override existing human-verified or rejected states</param>
    private void ProcessAllScores(Tournament tournament, bool overrideVerifiedState)
    {
        logger.LogDebug("Processing automation checks for all scores in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            foreach (Game game in match.Games)
            {
                foreach (GameScore score in game.Scores)
                {
                    // Check if the entity needs processing based on current verification status
                    if (!overrideVerifiedState &&
                        score.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
                    {
                        continue;
                    }

                    // Reset verification status and rejection reason if overriding
                    if (overrideVerifiedState &&
                        score.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
                    {
                        score.VerificationStatus = VerificationStatus.None;
                        score.RejectionReason = ScoreRejectionReason.None;
                    }

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
    /// <param name="tournament">The tournament containing the games to process</param>
    /// <param name="overrideVerifiedState">Whether to override existing human-verified or rejected states</param>
    private void ProcessAllGames(Tournament tournament, bool overrideVerifiedState)
    {
        logger.LogDebug("Processing automation checks for all games in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            foreach (Game game in match.Games)
            {
                // Check if the entity needs processing based on current verification status
                if (!overrideVerifiedState &&
                    game.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
                {
                    continue;
                }

                // Reset verification status and rejection reason if overriding
                if (overrideVerifiedState &&
                    game.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
                {
                    game.VerificationStatus = VerificationStatus.None;
                    game.WarningFlags = GameWarningFlags.None;
                    game.RejectionReason = GameRejectionReason.None;
                }

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
    /// <param name="tournament">The tournament containing the matches to process</param>
    /// <param name="overrideVerifiedState">Whether to override existing human-verified or rejected states</param>
    private void ProcessAllMatches(Tournament tournament, bool overrideVerifiedState)
    {
        logger.LogDebug("Processing automation checks for all matches in tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            // Check if the entity needs processing based on current verification status
            if (!overrideVerifiedState &&
                match.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
            {
                continue;
            }

            // Reset verification status and rejection reason if overriding
            if (overrideVerifiedState &&
                match.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
            {
                match.VerificationStatus = VerificationStatus.None;
                match.RejectionReason = MatchRejectionReason.None;
                match.WarningFlags = MatchWarningFlags.None;
            }

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

    /// <summary>
    /// Performs HeadToHead to TeamVs conversion for all matches in the tournament.
    /// This runs for all tournaments except those already Verified, ensuring data appears correctly on the website.
    /// Rejected tournaments still need conversion as they may have been rejected on submission.
    /// </summary>
    /// <param name="tournament">The tournament containing the matches to convert</param>
    private void PerformHeadToHeadConversion(Tournament tournament)
    {
        logger.LogDebug("Performing HeadToHead to TeamVs conversion for tournament {TournamentId}", tournament.Id);

        foreach (Match match in tournament.Matches)
        {
            // Always perform the conversion regardless of match verification status
            // This is critical for data display consistency on the website
            matchAutomationChecks.PerformHeadToHeadConversion(match, tournament);
        }
    }
}
