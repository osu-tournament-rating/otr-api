using System.Text.RegularExpressions;
using Common.Constants;
using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Utilities;
using DWS.Utilities.Extensions;
using Match = Database.Entities.Match;

namespace DWS.AutomationChecks;

/// <summary>
/// Performs automation checks for matches
/// </summary>
public class MatchAutomationChecks(ILogger<MatchAutomationChecks> logger)
{
    private const int HeadToHeadExpectedPlayerCount = 2;
    private const int HeadToHeadMaxScoresPerGame = 2;

    public MatchRejectionReason Process(Match match, Tournament tournament)
    {
        logger.LogTrace("Processing match {MatchId}", match.Id);

        // Head-to-head check is a conversion utility, modifies entities directly
        PerformHeadToHeadCheck(match, tournament);

        // Checks that modify warning flags
        MatchBeatmapCheck(match);
        MatchNameFormatCheck(match);
        MatchTeamsIntegrityCheck(match);

        // Checks that return rejection reasons
        MatchRejectionReason rejectionReason = MatchEndTimeCheck(match) |
                                             MatchGameCountCheck(match) | // Also modifies warning flags
                                             MatchNamePrefixCheck(match, tournament);

        if (rejectionReason != MatchRejectionReason.None || match.WarningFlags != MatchWarningFlags.None)
        {
            logger.LogTrace("Match {MatchId} processed with rejection reason: {RejectionReason} and warning flags: {WarningFlags}",
                match.Id, rejectionReason, match.WarningFlags);
        }

        return rejectionReason;
    }

    /// <summary>
    /// If the match has games beyond the first two in the collection
    /// which have a GameRejectionReason of BeatmapNotPooled, apply the
    /// UnexpectedBeatmapsFound WarningFlag to the match
    /// </summary>
    private static void MatchBeatmapCheck(Match match)
    {
        var games = match.Games.OrderBy(g => g.StartTime).ToList();
        if (games.Count < 2)
        {
            return;
        }

        // If any of Games 3 and beyond have a rejection reason of BeatmapNotPooled, return false
        if (games[2..].Any(g => g.RejectionReason.HasFlag(GameRejectionReason.BeatmapNotPooled)))
        {
            match.WarningFlags |= MatchWarningFlags.UnexpectedBeatmapsFound;
        }
    }

    /// <summary>
    /// Checks for matches where the EndTime could not be determined
    /// </summary>
    private static MatchRejectionReason MatchEndTimeCheck(Match match) =>
        match.EndTime is not null ? MatchRejectionReason.None : MatchRejectionReason.NoEndTime;

    /// <summary>
    /// Checks for matches with an unexpected count of valid games
    /// </summary>
    private static MatchRejectionReason MatchGameCountCheck(Match match)
    {
        // Match has no games at all
        if (match.Games.Count == 0)
        {
            return MatchRejectionReason.NoGames;
        }

        int validGamesCount = match.Games
                                   .Count(g => g.VerificationStatus.IsPreVerifiedOrVerified());

        switch (validGamesCount)
        {
            // Match has no valid games
            case 0:
                return MatchRejectionReason.NoValidGames;
            case < 3:
                return MatchRejectionReason.UnexpectedGameCount;
            case 3 or 4:
                match.WarningFlags |= MatchWarningFlags.LowGameCount;
                return MatchRejectionReason.None;
            // Number of games satisfies a "best of X" situation
            // This turned out to be not that worth to calculate, so as long as there are >= 3 games,
            // it is at least good enough to be sent to manual review
            default:
                return MatchRejectionReason.None;
        }
    }

    /// <summary>
    /// Checks for matches that have inconsistent lobby names
    /// </summary>
    private static void MatchNameFormatCheck(Match match)
    {
        if (!OtrConstants.MatchNamePatterns.Any(pattern => Regex.IsMatch(match.Name, pattern, RegexOptions.IgnoreCase)))
        {
            match.WarningFlags |= MatchWarningFlags.UnexpectedNameFormat;
        }
    }

    /// <summary>
    /// Checks for match names that do not begin with the parent tournament's abbreviation
    /// </summary>
    private static MatchRejectionReason MatchNamePrefixCheck(Match match, Tournament tournament) =>
        match.Name.StartsWith(tournament.Abbreviation, StringComparison.OrdinalIgnoreCase) ?
            MatchRejectionReason.None :
            MatchRejectionReason.NamePrefixMismatch;

    /// <summary>
    /// Checks for matches with the same player among teams
    /// </summary>
    private static void MatchTeamsIntegrityCheck(Match match)
    {
        Game[] validGames = match.Games.Where(g => g.VerificationStatus.IsPreVerifiedOrVerified()).ToArray();

        // Generate match rosters without modifying game entities
        ICollection<MatchRoster> matchRosters = RostersHelper.GenerateRosters(validGames);

        // Check for overlapping rosters
        HashSet<int>[] playerIdsPerRoster = matchRosters.Select(mr => mr.Roster.ToHashSet()).ToArray();

        for (int i = 0; i < playerIdsPerRoster.Length; i++)
        {
            for (int j = i + 1; j < playerIdsPerRoster.Length; j++)
            {
                if (!playerIdsPerRoster[i].Overlaps(playerIdsPerRoster[j]))
                {
                    continue;
                }

                match.WarningFlags |= MatchWarningFlags.OverlappingRosters;
                return;
            }
        }
    }

    /// <summary>
    /// Checks and converts matches from 1v1 tournaments where games were
    /// incorrectly set to HeadToHead instead of TeamVs.
    /// </summary>
    private void PerformHeadToHeadCheck(Match match, Tournament tournament)
    {
        if (!IsMatchEligibleForProcessing(match, tournament))
        {
            return;
        }

        var headToHeadGames = match.Games
                                   .Where(g => g.TeamType is TeamType.HeadToHead &&
                                               g.VerificationStatus != VerificationStatus.Rejected &&
                                               g.Scores.Count(s => s.VerificationStatus != VerificationStatus.Rejected) is 1 or HeadToHeadMaxScoresPerGame)
                                   .ToList();

        if (headToHeadGames.Count == 0)
        {
            logger.LogDebug("Match has no HeadToHead games eligible for TeamVs conversion");
            return;
        }

        logger.LogInformation("Attempting to convert HeadToHead games to TeamVs [Id: {Id}]", match.Id);

        var uniquePlayerIds = match.Games
                                   .SelectMany(g => g.Scores)
                                   .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
                                   .Select(s => s.Player.Id)
                                   .Distinct()
                                   .ToList();

        if (!ValidatePlayerCount(match, uniquePlayerIds, headToHeadGames))
        {
            return;
        }

        if (!ValidateGamePlayers(match, uniquePlayerIds))
        {
            return;
        }

        (int redPlayerId, int bluePlayerId) = DetermineTeamAssignments(match, uniquePlayerIds);
        ConvertGamesToTeamVs(headToHeadGames, redPlayerId, bluePlayerId);

        logger.LogInformation("Successfully converted HeadToHead games to TeamVs [Id: {Id}]", match.Id);
    }

    private bool IsMatchEligibleForProcessing(Match match, Tournament tournament)
    {
        if (match.Games.Count == 0)
        {
            logger.LogDebug("Match has no games");
            return false;
        }

        if (tournament.LobbySize == 1)
        {
            return true;
        }

        logger.LogDebug("Match's tournament team size is not 1 [Team size: {TeamSize}]", tournament.LobbySize);
        return false;
    }

    private bool ValidatePlayerCount(Match match, List<int> uniquePlayerIds, List<Game> headToHeadGames)
    {
        if (uniquePlayerIds.Count == HeadToHeadExpectedPlayerCount)
        {
            return true;
        }

        logger.LogInformation(
            "Match does not have exactly two unique participants, aborting TeamVs conversion [Id: {Id}, PlayerCount: {PlayerCount}]",
            match.Id,
            uniquePlayerIds.Count
        );

        match.RejectionReason |= MatchRejectionReason.FailedTeamVsConversion;
        foreach (Game game in headToHeadGames)
        {
            game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
        }

        return false;
    }

    private bool ValidateGamePlayers(Match match, List<int> uniquePlayerIds)
    {
        foreach (Game game in match.Games.Where(g => g.VerificationStatus != VerificationStatus.Rejected))
        {
            var gamePlayerIds = game.Scores
                                    .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
                                    .Select(s => s.Player.Id)
                                    .Distinct()
                                    .ToList();

            if (gamePlayerIds.All(uniquePlayerIds.Contains) && gamePlayerIds.Count <= HeadToHeadExpectedPlayerCount)
            {
                continue;
            }

            logger.LogInformation(
                "Game contains unexpected players or too many players, aborting TeamVs conversion [Id: {Id}, GameId: {GameId}]",
                match.Id,
                game.Id
            );

            game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
            game.RejectionReason |= GameRejectionReason.LobbySizeMismatch;
            return false;
        }

        return true;
    }

    private static (int redPlayerId, int bluePlayerId) DetermineTeamAssignments(Match match, List<int> uniquePlayerIds)
    {
        var allGames = match.Games.OrderBy(g => g.StartTime).ToList();
        int halfwayGameIndex = allGames.Count / 2;
        Game halfwayGame = allGames[halfwayGameIndex];

        var halfwayGamePlayerIds = halfwayGame.Scores
                                              .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
                                              .Select(s => s.Player.Id)
                                              .OrderBy(id => id)
                                              .ToList();

        return halfwayGamePlayerIds.Count switch
        {
            >= HeadToHeadExpectedPlayerCount => (halfwayGamePlayerIds[0], halfwayGamePlayerIds[1]),
            1 => (halfwayGamePlayerIds[0], uniquePlayerIds.First(id => id != halfwayGamePlayerIds[0])),
            _ => (uniquePlayerIds.OrderBy(id => id).First(), uniquePlayerIds.OrderBy(id => id).Last())
        };
    }

    private static void ConvertGamesToTeamVs(List<Game> headToHeadGames, int redPlayerId, int bluePlayerId)
    {
        foreach (Game game in headToHeadGames)
        {
            var scores = game.Scores
                             .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
                             .ToList();

            switch (scores.Count)
            {
                case HeadToHeadExpectedPlayerCount:
                    GameScore redScore = scores.First(s => s.Player.Id == redPlayerId);
                    GameScore blueScore = scores.First(s => s.Player.Id == bluePlayerId);
                    redScore.Team = Team.Red;
                    blueScore.Team = Team.Blue;
                    break;
                case 1:
                    scores[0].Team = scores[0].Player.Id == redPlayerId ? Team.Red : Team.Blue;
                    break;
            }

            game.TeamType = TeamType.TeamVs;
        }
    }
}
