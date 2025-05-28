using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;

namespace DataWorkerService.AutomationChecks.Matches;

/// <summary>
/// Checks (and attempts to fix) <see cref="Match"/>es played in a <see cref="Tournament"/> with a
/// <see cref="Tournament.LobbySize"/> of 1 where all <see cref="Match.Games"/> were played with a
/// <see cref="TeamType"/> of <see cref="TeamType.HeadToHead"/>
/// instead of <see cref="TeamType.TeamVs"/>
/// </summary>
/// <remarks>
/// Functionally this automation check attempts to programatically correct 1v1 Tournament games where the match was
/// mistakenly set to HeadToHead instead of TeamVs by assigning a team to both players
/// </remarks>
public class MatchHeadToHeadCheck(ILogger<MatchHeadToHeadCheck> logger) : AutomationCheckBase<Match>(logger)
{
    protected override bool OnChecking(Match entity)
    {
        if (entity.Games.Count == 0)
        {
            logger.LogDebug("Match has no games");
            return true;
        }

        if (entity.Tournament.LobbySize != 1)
        {
            logger.LogDebug("Match's tournament team size is not 1 [Team size: {TeamSize}]", entity.Tournament.LobbySize);
            return true;
        }

        // Find HeadToHead games with exactly 2 scores each
        var headToHeadGames = entity.Games
            .Where(g => g.TeamType is TeamType.HeadToHead && g.Scores.Count == 2)
            .ToList();

        if (headToHeadGames.Count == 0)
        {
            logger.LogDebug("Match has no HeadToHead games eligible for TeamVs conversion");
            return true;
        }

        logger.LogInformation("Attempting to convert HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        // Get all scores across all games in the match to determine participants
        var allMatchScores = entity.Games
            .SelectMany(g => g.Scores)
            .ToList();

        // Get unique player IDs across the entire match
        var uniquePlayerIds = allMatchScores
            .Select(s => s.Player.Id)
            .Distinct()
            .ToList();

        // Check if exactly two players participate throughout the whole match
        if (uniquePlayerIds.Count != 2)
        {
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

        // Verify that each game has exactly these two players
        foreach (Game game in entity.Games)
        {
            var gamePlayerIds = game.Scores
                .Select(s => s.Player.Id)
                .Distinct()
                .ToList();

            if (gamePlayerIds.Count != 2 || !gamePlayerIds.All(uniquePlayerIds.Contains))
            {
                logger.LogInformation(
                    "Game does not have the same two players as the match, aborting TeamVs conversion [Id: {Id}, GameId: {GameId}]",
                    entity.Id,
                    game.Id
                );

                entity.RejectionReason |= MatchRejectionReason.FailedTeamVsConversion;

                foreach (Game rejectedGame in headToHeadGames)
                {
                    rejectedGame.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
                }

                return false;
            }
        }

        // Get existing TeamVs games to determine most common team assignments
        var teamVsGames = entity.Games
            .Where(g => g.TeamType is TeamType.TeamVs && g.Scores.Count == 2)
            .OrderBy(g => g.Id) // Order by game ID to ensure consistent ordering
            .ToList();

        int redPlayerId;
        int bluePlayerId;

        if (teamVsGames.Count > 0)
        {
            // Count team assignments for each player across all TeamVs games
            var playerTeamCounts = uniquePlayerIds.ToDictionary(
                playerId => playerId,
                playerId => new { RedCount = 0, BlueCount = 0 }
            );

            foreach (var game in teamVsGames)
            {
                foreach (var score in game.Scores)
                {
                    var currentCounts = playerTeamCounts[score.Player.Id];
                    if (score.Team == Team.Red)
                    {
                        playerTeamCounts[score.Player.Id] = new { RedCount = currentCounts.RedCount + 1, currentCounts.BlueCount };
                    }
                    else if (score.Team == Team.Blue)
                    {
                        playerTeamCounts[score.Player.Id] = new { currentCounts.RedCount, BlueCount = currentCounts.BlueCount + 1 };
                    }
                }
            }

            // Determine team assignments based on most common colors
            int player1Id = uniquePlayerIds[0];
            int player2Id = uniquePlayerIds[1];

            var player1Counts = playerTeamCounts[player1Id];
            var player2Counts = playerTeamCounts[player2Id];

            // Check if there's a clear majority for each player
            // Require at least 2 games played to establish a clear preference
            int player1TotalGames = player1Counts.RedCount + player1Counts.BlueCount;
            int player2TotalGames = player2Counts.RedCount + player2Counts.BlueCount;

            bool player1HasClearPreference = player1TotalGames >= 2 && (player1Counts.RedCount > player1Counts.BlueCount || player1Counts.BlueCount > player1Counts.RedCount);
            bool player2HasClearPreference = player2TotalGames >= 2 && (player2Counts.RedCount > player2Counts.BlueCount || player2Counts.BlueCount > player2Counts.RedCount);

            bool player1PrefersRed = player1HasClearPreference && player1Counts.RedCount > player1Counts.BlueCount;
            bool player1PrefersBlue = player1HasClearPreference && player1Counts.BlueCount > player1Counts.RedCount;
            bool player2PrefersRed = player2HasClearPreference && player2Counts.RedCount > player2Counts.BlueCount;
            bool player2PrefersBlue = player2HasClearPreference && player2Counts.BlueCount > player2Counts.RedCount;

            if ((player1PrefersRed && player2PrefersBlue) || (player1PrefersBlue && player2PrefersRed))
            {
                // Clear assignment based on preferences
                redPlayerId = player1PrefersRed ? player1Id : player2Id;
                bluePlayerId = player1PrefersBlue ? player1Id : player2Id;
            }
            else
            {
                // Handle ties or conflicting preferences - use second-to-last game if possible
                if (teamVsGames.Count >= 2)
                {
                    var secondToLastGame = teamVsGames[teamVsGames.Count - 2];
                    var player1ScoreInSecondToLast = secondToLastGame.Scores.First(s => s.Player.Id == player1Id);

                    redPlayerId = player1ScoreInSecondToLast.Team == Team.Red ? player1Id : player2Id;
                    bluePlayerId = player1ScoreInSecondToLast.Team == Team.Blue ? player1Id : player2Id;
                }
                else
                {
                    // Cannot determine team assignment, fail conversion
                    logger.LogInformation(
                        "Cannot determine consistent team assignment for HeadToHead conversion, aborting [Id: {Id}]",
                        entity.Id
                    );

                    entity.RejectionReason |= MatchRejectionReason.FailedTeamVsConversion;

                    foreach (Game game in headToHeadGames)
                    {
                        game.RejectionReason |= GameRejectionReason.FailedTeamVsConversion;
                    }

                    return false;
                }
            }
        }
        else
        {
            // No existing TeamVs games, fall back to total score method
            var playerTotalScores = uniquePlayerIds.ToDictionary(
                playerId => playerId,
                playerId => allMatchScores
                    .Where(s => s.Player.Id == playerId)
                    .Sum(s => s.Score)
            );

            redPlayerId = playerTotalScores
                .OrderByDescending(kvp => kvp.Value)
                .First()
                .Key;
            bluePlayerId = uniquePlayerIds.First(id => id != redPlayerId);
        }

        // Convert only the HeadToHead games to TeamVs by assigning teams
        foreach (Game game in headToHeadGames)
        {
            var scores = game.Scores.ToList();

            GameScore redScore = scores.First(s => s.Player.Id == redPlayerId);
            GameScore blueScore = scores.First(s => s.Player.Id == bluePlayerId);

            redScore.Team = Team.Red;
            blueScore.Team = Team.Blue;

            game.TeamType = TeamType.TeamVs;
        }

        logger.LogInformation("Successfully converted HeadToHead games to TeamVs [Id: {Id}]", entity.Id);

        return true;
    }
}
