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
    public async Task<bool> ProcessAutomationChecksAsync(int tournamentId, bool overrideVerifiedState = false)
    {
        logger.LogInformation("Processing automation checks for tournament {TournamentId}", tournamentId);

        // Fetch the tournament with eager loading of navigation properties
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId, eagerLoad: true);
        if (tournament is null)
        {
            logger.LogWarning("Tournament {TournamentId} not found", tournamentId);
            return false;
        }

        VerificationStatus tournamentStatusBefore = tournament.VerificationStatus;

        await tournamentsRepository.LoadMatchesWithGamesAndScoresAsync(tournament);

        // Ordering matters - HeadToHead conversion must be performed before
        // rejecting all child entities.
        if (tournament.VerificationStatus != VerificationStatus.Verified)
        {
            PerformHeadToHeadConversion(tournament);
        }

        if (tournament.VerificationStatus == VerificationStatus.Rejected)
        {
            logger.LogInformation(
                "Cascading rejection status to child entities for tournament {TournamentId} after HeadToHead conversion",
                tournamentId);
            tournament.RejectAllChildren();
        }

        bool skipAutomationChecks = !overrideVerifiedState &&
            tournament.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected;

        if (skipAutomationChecks)
        {
            logger.LogInformation(
                "Skipping automation checks for tournament {TournamentId} with verification status {VerificationStatus}",
                tournamentId,
                tournament.VerificationStatus);

            if (tournament.VerificationStatus == VerificationStatus.Rejected)
            {
                await tournamentsRepository.UpdateAsync(tournament);
            }

            return tournament.VerificationStatus == VerificationStatus.Verified;
        }

        if (overrideVerifiedState)
        {
            tournament.VerificationStatus = VerificationStatus.None;
            tournament.RejectionReason = TournamentRejectionReason.None;
            logger.LogInformation(
                "Resetting verification status to None for tournament {TournamentId} due to override",
                tournamentId);
        }

        ProcessAllScores(tournament, overrideVerifiedState);
        ProcessAllGames(tournament, overrideVerifiedState);
        ProcessAllMatches(tournament, overrideVerifiedState);

        TournamentRejectionReason rejectionReason = tournamentAutomationChecks.Process(tournament);

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

        TournamentProcessingState processingState = TournamentProcessingReporter.CaptureDetailedState(tournament, tournamentStatusBefore);

        await tournamentsRepository.UpdateAsync(tournament);

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
                    if (!overrideVerifiedState &&
                        score.VerificationStatus is VerificationStatus.Verified or VerificationStatus.Rejected)
                    {
                        continue;
                    }

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
