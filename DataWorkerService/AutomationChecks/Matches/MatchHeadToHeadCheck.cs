using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks and converts <see cref="Match"/>es from 1v1 tournaments where <see cref="Game"/>s were 
/// incorrectly set to <see cref="TeamType.HeadToHead"/> instead of <see cref="TeamType.TeamVs"/>.
/// Only processes matches from tournaments with a <see cref="Tournament.LobbySize"/> of 1.
/// </summary>
/// <remarks>
/// This automation check identifies matches where HeadToHead games should be converted to TeamVs format
/// for proper 1v1 tournament processing. The conversion process:
/// <list type="bullet">
/// <item>Validates that exactly 2 unique players participated across all games in the match</item>
/// <item>Ensures all games contain only players from the validated player set</item>
/// <item>Determines team assignments based on player order in the "halfway" game (middle game by startTime)</item>
/// <item>Converts eligible HeadToHead games (1-2 scores) to TeamVs with Red/Blue team assignments</item>
/// <item>Supports games where one player disconnected (single score games)</item>
/// </list>
/// If validation fails, sets appropriate rejection reasons on the match and affected games.
/// </remarks>
public class MatchHeadToHeadCheck(ILogger<MatchHeadToHeadCheck> logger) : AutomationCheckBase<Match>(logger)
{
    private const int ExpectedPlayerCount = 2;
    private const int MaxScoresPerGame = 2;

    protected override bool OnChecking(Match entity)
    {
        if (!IsMatchEligibleForProcessing(entity))
        {
            return true;
        }

        var headToHeadGames = entity.Games
            .Where(g => g.TeamType is TeamType.HeadToHead &&
                       g.VerificationStatus != VerificationStatus.Rejected &&
                       g.Scores.Count(s => s.VerificationStatus != VerificationStatus.Rejected) is 1 or MaxScoresPerGame)
            .ToList();

        if (headToHeadGames.Count == 0)
        {
            logger.LogDebug("Match has no HeadToHead games eligible for TeamVs conversion");
            return true;
        }

        logger.LogInformation("Attempting to convert HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        var uniquePlayerIds = entity.Games
            .SelectMany(g => g.Scores)
            .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
            .Select(s => s.Player.Id)
            .Distinct()
            .ToList();

        if (!ValidatePlayerCount(entity, uniquePlayerIds, headToHeadGames))
        {
            return false;
        }

        if (!ValidateGamePlayers(entity, uniquePlayerIds))
        {
            return false;
        }

        (int redPlayerId, int bluePlayerId) = DetermineTeamAssignments(entity, uniquePlayerIds);
        ConvertGamesToTeamVs(headToHeadGames, redPlayerId, bluePlayerId);

        logger.LogInformation("Successfully converted HeadToHead games to TeamVs [Id: {Id}]", entity.Id);
        return true;
    }

    private bool IsMatchEligibleForProcessing(Match entity)
    {
        if (entity.Games.Count == 0)
        {
            logger.LogDebug("Match has no games");
            return false;
        }

        if (entity.Tournament.LobbySize == 1)
        {
            return true;
        }

        logger.LogDebug("Match's tournament team size is not 1 [Team size: {TeamSize}]", entity.Tournament.LobbySize);
        return false;
    }

    private bool ValidatePlayerCount(Match entity, List<int> uniquePlayerIds, List<Game> headToHeadGames)
    {
        if (uniquePlayerIds.Count == ExpectedPlayerCount)
        {
            return true;
        }

        logger.LogInformation(
            "Match does not have exactly two unique participants, aborting TeamVs conversion [Id: {Id}, PlayerCount: {PlayerCount}]",
            entity.Id,
            uniquePlayerIds.Count
        );

        entity.RejectionReason |= MatchRejectionReason.FailedTeamVsConversion;
        foreach (Game game in headToHeadGames)
        {
            game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
        }

        return false;
    }

    private bool ValidateGamePlayers(Match entity, List<int> uniquePlayerIds)
    {
        foreach (Game game in entity.Games.Where(g => g.VerificationStatus != VerificationStatus.Rejected))
        {
            var gamePlayerIds = game.Scores
                .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
                .Select(s => s.Player.Id)
                .Distinct()
                .ToList();

            if (gamePlayerIds.All(uniquePlayerIds.Contains) && gamePlayerIds.Count <= ExpectedPlayerCount)
            {
                continue;
            }

            logger.LogInformation(
                "Game contains unexpected players or too many players, aborting TeamVs conversion [Id: {Id}, GameId: {GameId}]",
                entity.Id,
                game.Id
            );

            game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
            game.RejectionReason |= GameRejectionReason.LobbySizeMismatch;
            return false;
        }

        return true;
    }

    private static (int redPlayerId, int bluePlayerId) DetermineTeamAssignments(Match entity, List<int> uniquePlayerIds)
    {
        var allGames = entity.Games.OrderBy(g => g.StartTime).ToList();
        int halfwayGameIndex = allGames.Count / 2;
        Game halfwayGame = allGames[halfwayGameIndex];

        var halfwayGamePlayerIds = halfwayGame.Scores
            .Where(s => s.VerificationStatus != VerificationStatus.Rejected)
            .Select(s => s.Player.Id)
            .OrderBy(id => id)
            .ToList();

        return halfwayGamePlayerIds.Count switch
        {
            >= ExpectedPlayerCount => (halfwayGamePlayerIds[0], halfwayGamePlayerIds[1]),
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
                case ExpectedPlayerCount:
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
