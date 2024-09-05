using System.Diagnostics.CodeAnalysis;
using Database.Entities;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;

namespace DataWorkerService.Utilities;

/// <summary>
/// Calculates Match Costs for a <see cref="Database.Entities.Match"/>
/// </summary>
public static class MatchCostCalculator
{
    /// <summary>
    /// Calculates the match cost for each player in a given list of games using the o!TR Match Cost formula
    /// </summary>
    /// <param name="games">Games to calculate match costs for</param>
    /// <returns>A dictionary of <see cref="Player"/>.<see cref="Player.Id"/>, match cost</returns>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static IDictionary<int, double> CalculateOtrMatchCosts(IEnumerable<Game> games)
    {
        IEnumerable<Game> enumerableGames = games as Game[] ?? games.ToArray();

        var zScores = enumerableGames
            .SelectMany(g => g.Scores)
            .Select(s => s.Player.Id)
            .Distinct()
            .Select(id => new KeyValuePair<int, List<double>>(id, []))
            .ToDictionary();

        foreach (Game game in enumerableGames)
        {
            var gameScoresAvg = game.Scores.Average(s => s.Score);
            var gameScoresStdev = game.Scores.Select(s => (double)s.Score).StandardDeviation();

            // Calculate z-scores
            foreach (GameScore score in game.Scores)
            {
                zScores[score.Player.Id].Add((score.Score - gameScoresAvg) / gameScoresStdev);
            }
        }

        // Create a normal distribution with z-score mean (mu) and z-score stdev (sigma)
        var zScoresNormal = new Normal(
            zScores.SelectMany(pair => pair.Value).Mean(),
            zScores.SelectMany(pair => pair.Value).StandardDeviation()
        );

        var gamesCount = enumerableGames.Count();

        // Calculate match costs
        return zScores.Select(pair =>
            new KeyValuePair<int, double>(
                pair.Key,
                // Average z-score percentile
                (zScoresNormal.CumulativeDistribution(pair.Value.Average()) + 0.5)
                *
                // Performance bonus
                (1 + 0.3 * Math.Sqrt((enumerableGames.Count(g => g.Scores.Any(s => s.Player.Id == pair.Key)) - 1) - (gamesCount - 1)))
            )
        ).ToDictionary();
    }
}
