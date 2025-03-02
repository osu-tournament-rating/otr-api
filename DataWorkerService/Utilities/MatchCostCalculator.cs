using Database.Entities;
using Database.Enums.Verification;
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
    /// <returns>A dictionary of { <see cref="Player"/>.<see cref="Player.Id"/>, match cost }</returns>
    /// <remarks>
    /// Only <see cref="GameScore"/>s with a <see cref="VerificationStatus"/> of <see cref="VerificationStatus.Verified"/>
    /// and a <see cref="ScoreProcessingStatus"/> of <see cref="ScoreProcessingStatus.Done"/> are considered
    /// </remarks>
    public static IDictionary<int, double> CalculateOtrMatchCosts(IEnumerable<Game> games)
    {
        games = (List<Game>)[.. games];

        var zScores = games
            .SelectMany(g => g.Scores)
            .WhereValid()
            .Select(s => s.Player.Id)
            .Distinct()
            .Select(id => new KeyValuePair<int, List<double>>(id, []))
            .ToDictionary();

        foreach (Game game in games)
        {
            var scores = game.Scores.WhereValid().ToList();

            var gameScoresAvg = scores.Average(s => s.Score);
            var gameScoresStdev = scores.Select(s => (double)s.Score).StandardDeviation();

            // Calculate z-scores
            foreach (GameScore score in scores)
            {
                zScores[score.Player.Id].Add((score.Score - gameScoresAvg) / gameScoresStdev);
            }
        }

        var normal = new Normal(0, 1);
        var gamesCount = games.Count();

        // Calculate match costs
        return zScores.Select(pair =>
            new KeyValuePair<int, double>(
                pair.Key,
                // Average z-score percentile
                (pair.Value.Select(normal.CumulativeDistribution).Average() + 0.5)
                *
                // Performance bonus
                (1 + 0.3 * Math.Sqrt((pair.Value.Count - 1) / (double)(gamesCount - 1)))
            )
        ).ToDictionary();
    }

    private static IEnumerable<GameScore> WhereValid(this IEnumerable<GameScore> scores) =>
        scores.Where(s => s is { VerificationStatus: VerificationStatus.Verified, ProcessingStatus: ScoreProcessingStatus.Done });
}
