using Common.Enums.Verification;
using Database.Entities;

namespace DWS.AutomationChecks;

/// <summary>
/// Processes automation checks for tournaments to determine their verification status
/// </summary>
public class TournamentAutomationChecks(ILogger<TournamentAutomationChecks> logger)
{
    /// <summary>
    /// Percentage threshold (0.0 to 1.0) that a tournament's verified matches must meet or exceed
    /// </summary>
    private const double TournamentVerifiedMatchesPercentageThreshold = 0.8;

    /// <summary>
    /// Process automation checks for a tournament
    /// </summary>
    /// <param name="tournament">The tournament to check</param>
    /// <returns>The rejection reason, or None if the tournament passes all checks</returns>
    public TournamentRejectionReason Process(Tournament tournament)
    {
        logger.LogTrace("Processing tournament {TournamentId}", tournament.Id);

        TournamentRejectionReason rejectionReason = VerifiedMatchesCheck(tournament);

        logger.LogTrace("Tournament {TournamentId} processed with rejection reason: {RejectionReason}",
            tournament.Id, rejectionReason);
        return rejectionReason;
    }

    /// <summary>
    /// Checks if the tournament has sufficient verified matches
    /// </summary>
    /// <param name="tournament">The tournament to check</param>
    /// <returns>The rejection reason based on match verification status</returns>
    private TournamentRejectionReason VerifiedMatchesCheck(Tournament tournament)
    {
        // Get matches that have games (matches without games are excluded from percentage calculation)
        var matchesWithGames = tournament.Matches
            .Where(m => m.Games.Count != 0)
            .ToList();

        if (matchesWithGames.Count == 0)
        {
            logger.LogTrace("Tournament {TournamentId} has no matches with games", tournament.Id);
            return TournamentRejectionReason.NoVerifiedMatches;
        }

        int verifiedMatches = matchesWithGames
            .Count(m => m.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified);

        if (verifiedMatches == 0)
        {
            logger.LogTrace("Tournament {TournamentId} has no verified matches", tournament.Id);
            return TournamentRejectionReason.NoVerifiedMatches;
        }

        double verificationPercentage = (double)verifiedMatches / matchesWithGames.Count;

        if (verificationPercentage < TournamentVerifiedMatchesPercentageThreshold)
        {
            logger.LogTrace(
                "Tournament {TournamentId} has insufficient verified matches. " +
                "Verified: {VerifiedCount}/{TotalCount} ({Percentage:P2}), Required: {Threshold:P2}",
                tournament.Id, verifiedMatches, matchesWithGames.Count,
                verificationPercentage, TournamentVerifiedMatchesPercentageThreshold);
            return TournamentRejectionReason.NotEnoughVerifiedMatches;
        }

        logger.LogTrace(
            "Tournament {TournamentId} has sufficient verified matches. " +
            "Verified: {VerifiedCount}/{TotalCount} ({Percentage:P2})",
            tournament.Id, verifiedMatches, matchesWithGames.Count, verificationPercentage);
        return TournamentRejectionReason.None;
    }
}
