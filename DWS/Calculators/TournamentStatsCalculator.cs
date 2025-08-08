using Common.Enums;
using Common.Enums.Verification;
using Database.Entities;
using DWS.Utilities;

namespace DWS.Calculators;

/// <summary>
/// Functional calculator for tournament statistics that performs all calculations in-memory.
/// </summary>
public class TournamentStatsCalculator(
    ILogger<TournamentStatsCalculator> logger) : IStatsCalculator
{
    /// <inheritdoc />
    public bool CalculateAllStatistics(Tournament tournament)
    {
        if (tournament.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogError(
                "Stats calculation attempted for unverified tournament [Id: {Id} | Status: {Status}]",
                tournament.Id,
                tournament.VerificationStatus
            );
            return false;
        }

        // Filter to only verified matches
        List<Match> verifiedMatches = [.. tournament.Matches
            .Where(m => m.VerificationStatus == VerificationStatus.Verified)];

        if (verifiedMatches.Count == 0)
        {
            logger.LogWarning(
                "No verified matches found for tournament [Id: {Id}]",
                tournament.Id
            );
            return false;
        }

        // Process all matches and their games
        foreach (Match match in verifiedMatches.Where(match => !ProcessMatchStatistics(match)))
        {
            logger.LogError(
                "Failed to process match statistics [Match Id: {MatchId} | Tournament Id: {TournamentId}]",
                match.Id,
                tournament.Id
            );
            return false;
        }

        // Validate processor data exists
        if (!ValidateProcessorData(verifiedMatches))
        {
            return false;
        }

        // Aggregate player tournament statistics
        AggregatePlayerTournamentStatistics(tournament, verifiedMatches);

        // Update processing timestamp
        tournament.LastProcessingDate = DateTime.UtcNow;

        logger.LogInformation(
            "Successfully calculated tournament statistics [Id: {Id} | Matches: {MatchCount} | Players: {PlayerCount}]",
            tournament.Id,
            verifiedMatches.Count,
            tournament.PlayerTournamentStats.Count
        );

        return true;
    }

    /// <summary>
    /// Processes statistics for a single match and all its games.
    /// </summary>
    /// <param name="match">The match to process.</param>
    /// <returns>True if processing succeeded, false otherwise.</returns>
    private bool ProcessMatchStatistics(Match match)
    {
        if (match.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogError(
                "Attempted to process unverified match [Id: {Id} | Status: {Status}]",
                match.Id,
                match.VerificationStatus
            );
            return false;
        }

        List<Game> verifiedGames = [.. match.Games
            .Where(g => g.VerificationStatus == VerificationStatus.Verified)];

        if (verifiedGames.Count == 0)
        {
            logger.LogError(
                "No verified games found for match [Id: {Id}]",
                match.Id
            );
            return false;
        }

        // Process each game's statistics
        foreach (Game game in verifiedGames)
        {
            ProcessGameStatistics(game);
        }

        // Validate all games have rosters
        if (verifiedGames.Any(g => g.Rosters.Count == 0))
        {
            logger.LogWarning(
                "One or more games missing rosters after processing [Match Id: {Id}]",
                match.Id
            );
            return false;
        }

        // Generate match rosters from games
        match.Rosters.Clear();
        ICollection<MatchRoster> newMatchRosters = RostersHelper.GenerateRosters(verifiedGames);
        foreach (MatchRoster roster in newMatchRosters)
        {
            match.Rosters.Add(roster);
        }

        // Generate player match statistics
        match.PlayerMatchStats.Clear();
        IEnumerable<PlayerMatchStats> playerStats = GeneratePlayerMatchStatistics(verifiedGames);
        foreach (PlayerMatchStats stat in playerStats)
        {
            match.PlayerMatchStats.Add(stat);
        }

        match.LastProcessingDate = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Processes statistics for a single game.
    /// </summary>
    /// <param name="game">The game to process.</param>
    private void ProcessGameStatistics(Game game)
    {
        if (game.VerificationStatus is not VerificationStatus.Verified)
        {
            logger.LogWarning(
                "Skipping unverified game [Id: {Id} | Status: {Status}]",
                game.Id,
                game.VerificationStatus
            );
            return;
        }

        List<GameScore> verifiedScores = [.. game.Scores
            .Where(s => s.VerificationStatus == VerificationStatus.Verified)
            .OrderByDescending(s => s.Score)];

        // Assign placements
        int placement = 1;
        foreach (GameScore score in verifiedScores)
        {
            score.Placement = placement++;
        }

        // Generate game rosters
        game.Rosters.Clear();
        ICollection<GameRoster> newGameRosters = RostersHelper.GenerateRosters(verifiedScores);
        foreach (GameRoster roster in newGameRosters)
        {
            game.Rosters.Add(roster);
        }

        game.LastProcessingDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Generates player match statistics from verified games.
    /// </summary>
    /// <param name="games">The games to generate statistics from.</param>
    /// <returns>Collection of player match statistics.</returns>
    private IEnumerable<PlayerMatchStats> GeneratePlayerMatchStatistics(List<Game> games)
    {
        if (games.Count == 0)
        {
            return [];
        }

        // Validate all games have rosters and belong to same match
        if (games.Any(g => g.Rosters.Count == 0))
        {
            logger.LogWarning("One or more games have empty rosters");
            return [];
        }

        int matchId = games.First().MatchId;
        if (games.Any(g => g.MatchId != matchId))
        {
            logger.LogError("Games belong to different matches");
            return [];
        }

        // Calculate match costs
        IDictionary<int, double> matchCosts = MatchCostCalculator.CalculateOtrMatchCosts(games);

        // Pre-calculate winning teams for performance
        Dictionary<int, Team?> gameWinningTeams = CalculateGameWinningTeams(games);

        // Get match rosters and max score
        ICollection<MatchRoster> matchRosters = games.First().Match.Rosters;
        int maxMatchScore = matchRosters.Count > 0 ? matchRosters.Max(r => r.Score) : 0;

        // Group scores by player
        var playerScoreGroups = games
            .SelectMany(g => g.Scores)
            .Where(s => s.VerificationStatus == VerificationStatus.Verified)
            .GroupBy(s => s.PlayerId)
            .Select(group => new
            {
                PlayerId = group.Key,
                Scores = group.ToList()
            })
            .ToList();

        return playerScoreGroups.Select(playerData =>
        {
            var playerGames = playerData.Scores.Select(s => s.Game).Distinct().ToList();

            // Calculate games won/lost
            int gamesWon = 0;
            int gamesLost = 0;
            foreach (Game game in playerGames)
            {
                Team playerTeam = playerData.Scores.First(s => s.GameId == game.Id).Team;
                Team? winningTeam = gameWinningTeams.GetValueOrDefault(game.Id);

                if (winningTeam == playerTeam)
                {
                    gamesWon++;
                }
                else if (winningTeam != null)
                {
                    gamesLost++;
                }
            }

            // Determine match outcome
            MatchRoster? playerMatchRoster = matchRosters.FirstOrDefault(r => r.Roster.Contains(playerData.PlayerId));
            bool matchWon = playerMatchRoster != null && playerMatchRoster.Score == maxMatchScore;

            // Get teammates and opponents
            List<int> teammateIds = playerMatchRoster?.Roster.Where(id => id != playerData.PlayerId).ToList() ?? [];
            var opponentIds = matchRosters
                .Where(r => r.Team != playerMatchRoster?.Team)
                .SelectMany(r => r.Roster)
                .Distinct()
                .Where(id => id != playerData.PlayerId)
                .ToList();

            return new PlayerMatchStats
            {
                PlayerId = playerData.PlayerId,
                MatchId = matchId,
                MatchCost = matchCosts.TryGetValue(playerData.PlayerId, out double mc) ? mc : 0,
                AverageScore = playerData.Scores.Average(s => s.Score),
                AveragePlacement = playerData.Scores.Average(s => s.Placement),
                AverageMisses = playerData.Scores.Average(s => s.CountMiss),
                AverageAccuracy = playerData.Scores.Average(s => s.Accuracy),
                GamesPlayed = playerData.Scores.Count,
                GamesWon = gamesWon,
                GamesLost = gamesLost,
                Won = matchWon,
                TeammateIds = [.. teammateIds],
                OpponentIds = [.. opponentIds]
            };
        });
    }

    /// <summary>
    /// Calculates the winning team for each game.
    /// </summary>
    /// <param name="games">The games to analyze.</param>
    /// <returns>Dictionary mapping game ID to winning team.</returns>
    private static Dictionary<int, Team?> CalculateGameWinningTeams(List<Game> games)
    {
        var result = new Dictionary<int, Team?>();

        foreach (Game game in games)
        {
            var teamScores = game.Scores
                .Where(s => s.VerificationStatus == VerificationStatus.Verified)
                .GroupBy(s => s.Team)
                .Select(g => new
                {
                    Team = g.Key,
                    TotalScore = g.Sum(s => s.Score)
                })
                .OrderByDescending(ts => ts.TotalScore)
                .ToList();

            result[game.Id] = teamScores.FirstOrDefault()?.Team;
        }

        return result;
    }

    /// <summary>
    /// Validates that all matches have required processor data.
    /// </summary>
    /// <param name="matches">The matches to validate.</param>
    /// <returns>True if all matches have processor data, false otherwise.</returns>
    private bool ValidateProcessorData(List<Match> matches)
    {
        foreach (Match match in matches.Where(match => match.PlayerRatingAdjustments.Count == 0))
        {
            logger.LogWarning(
                "Match missing rating adjustments from processor [Match Id: {Id}]",
                match.Id
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Aggregates player statistics across all tournament matches.
    /// </summary>
    /// <param name="tournament">The tournament to update.</param>
    /// <param name="verifiedMatches">The verified matches to aggregate from.</param>
    private void AggregatePlayerTournamentStatistics(Tournament tournament, List<Match> verifiedMatches)
    {
        // Clear existing statistics
        tournament.PlayerTournamentStats.Clear();

        // Pre-aggregate all player data for efficiency
        var playerDataGroups = verifiedMatches
            .SelectMany(m => m.PlayerMatchStats.Select(pms => new
            {
                MatchStats = pms,
                RatingAdjustments = m.PlayerRatingAdjustments.Where(ra => ra.PlayerId == pms.PlayerId).ToList()
            }))
            .Where(x => x.RatingAdjustments.Count > 0) // Skip restricted players
            .GroupBy(x => x.MatchStats.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                MatchStatsList = g.Select(x => x.MatchStats).ToList(),
                AllRatingAdjustments = g.SelectMany(x => x.RatingAdjustments).ToArray()
            })
            .ToList();

        foreach (var playerData in playerDataGroups)
        {
            if (playerData.AllRatingAdjustments.Length == 0)
            {
                logger.LogDebug(
                    "Skipping player with no rating adjustments [Player Id: {PlayerId} | Tournament Id: {TournamentId}]",
                    playerData.PlayerId,
                    tournament.Id
                );
                continue;
            }

            var stats = new PlayerTournamentStats
            {
                PlayerId = playerData.PlayerId,
                TournamentId = tournament.Id,
                AverageRatingDelta = playerData.AllRatingAdjustments.Average(ra => ra.RatingDelta),
                AverageMatchCost = playerData.MatchStatsList.Average(pms => pms.MatchCost),
                AverageScore = (int)playerData.MatchStatsList.Average(pms => pms.AverageScore),
                AveragePlacement = playerData.MatchStatsList.Average(pms => pms.AveragePlacement),
                AverageAccuracy = playerData.MatchStatsList.Average(pms => pms.AverageAccuracy),
                MatchesPlayed = playerData.MatchStatsList.Count,
                MatchesWon = playerData.MatchStatsList.Count(pms => pms.Won),
                MatchesLost = playerData.MatchStatsList.Count(pms => !pms.Won),
                GamesPlayed = playerData.MatchStatsList.Sum(pms => pms.GamesPlayed),
                GamesWon = playerData.MatchStatsList.Sum(pms => pms.GamesWon),
                GamesLost = playerData.MatchStatsList.Sum(pms => pms.GamesLost),
                MatchWinRate = playerData.MatchStatsList.Count(pms => pms.Won) / (double)playerData.MatchStatsList.Count,
                TeammateIds = [.. playerData.MatchStatsList.SelectMany(pms => pms.TeammateIds).Distinct()]
            };

            tournament.PlayerTournamentStats.Add(stats);
        }
    }
}
