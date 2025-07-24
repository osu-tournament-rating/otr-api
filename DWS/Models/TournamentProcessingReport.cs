using System.Text;
using Common.Enums.Verification;
using Database.Entities;

namespace DWS.Models;

/// <summary>
/// Groups rejection details by reason
/// </summary>
public class RejectionGroup
{
    /// <summary>
    /// The rejection reason
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Number of entities with this rejection reason
    /// </summary>
    public int Count { get; init; }
}

/// <summary>
/// Represents verification counts for entities
/// </summary>
public readonly struct VerificationCounts
{
    /// <summary>
    /// Number of pre-verified entities
    /// </summary>
    public int PreVerified { get; init; }

    /// <summary>
    /// Number of verified entities
    /// </summary>
    public int Verified { get; init; }

    /// <summary>
    /// Number of pre-rejected entities
    /// </summary>
    public int PreRejected { get; init; }

    /// <summary>
    /// Number of rejected entities
    /// </summary>
    public int Rejected { get; init; }
}

/// <summary>
/// Captures the detailed state of a tournament and its entities for reporting purposes
/// </summary>
public class TournamentProcessingState
{
    /// <summary>
    /// The tournament ID
    /// </summary>
    public int TournamentId { get; init; }

    /// <summary>
    /// The tournament abbreviation
    /// </summary>
    public string Abbreviation { get; init; } = string.Empty;

    /// <summary>
    /// The tournament verification status before processing
    /// </summary>
    public VerificationStatus TournamentStatusBefore { get; init; }

    /// <summary>
    /// The tournament verification status after processing
    /// </summary>
    public VerificationStatus TournamentStatusAfter { get; init; }

    /// <summary>
    /// The tournament rejection reason
    /// </summary>
    public TournamentRejectionReason TournamentRejectionReason { get; init; }

    /// <summary>
    /// Match rejection details grouped by reason
    /// </summary>
    public List<RejectionGroup> MatchRejections { get; init; } = new();

    /// <summary>
    /// Game rejection details grouped by reason
    /// </summary>
    public List<RejectionGroup> GameRejections { get; init; } = new();

    /// <summary>
    /// Score rejection details grouped by reason
    /// </summary>
    public List<RejectionGroup> ScoreRejections { get; init; } = new();

    /// <summary>
    /// Total number of matches in the tournament
    /// </summary>
    public int TotalMatches { get; init; }

    /// <summary>
    /// Total number of games in the tournament
    /// </summary>
    public int TotalGames { get; init; }

    /// <summary>
    /// Total number of scores in the tournament
    /// </summary>
    public int TotalScores { get; init; }

    /// <summary>
    /// Number of matches with warning flags
    /// </summary>
    public int MatchesWithWarnings { get; init; }

    /// <summary>
    /// Number of games with warning flags
    /// </summary>
    public int GamesWithWarnings { get; init; }

    /// <summary>
    /// Verification counts for matches
    /// </summary>
    public VerificationCounts MatchCounts { get; init; }

    /// <summary>
    /// Verification counts for games
    /// </summary>
    public VerificationCounts GameCounts { get; init; }

    /// <summary>
    /// Verification counts for scores
    /// </summary>
    public VerificationCounts ScoreCounts { get; init; }
}

/// <summary>
/// Generates processing reports for tournaments
/// </summary>
public static class TournamentProcessingReporter
{
    /// <summary>
    /// Column separator for report formatting
    /// </summary>
    private const string ColumnSeparator = "\t";

    /// <summary>
    /// Field separator within columns
    /// </summary>
    private const string FieldSeparator = " | ";

    /// <summary>
    /// Label for tournament section in report
    /// </summary>
    private const string TournamentLabel = "Tournament";

    /// <summary>
    /// Label for matches section in report
    /// </summary>
    private const string MatchesLabel = "Matches";

    /// <summary>
    /// Label for games section in report
    /// </summary>
    private const string GamesLabel = "Games";

    /// <summary>
    /// Label for scores section in report
    /// </summary>
    private const string ScoresLabel = "Scores";

    /// <summary>
    /// Label for summary section in report
    /// </summary>
    private const string SummaryLabel = "Summary";

    /// <summary>
    /// Captures the detailed state of a tournament and its entities
    /// </summary>
    /// <param name="tournament">The tournament to capture state from</param>
    /// <param name="beforeStatus">The tournament status before processing</param>
    /// <returns>The captured state</returns>
    /// <exception cref="ArgumentNullException">Thrown when tournament is null</exception>
    public static TournamentProcessingState CaptureDetailedState(
        Tournament tournament,
        VerificationStatus beforeStatus)
    {
        ArgumentNullException.ThrowIfNull(tournament);

        List<Match> matches = tournament.Matches.ToList();
        var games = new List<Game>();
        var scores = new List<GameScore>();

        foreach (Game game in matches.Where(match => match.Games.Count != 0).SelectMany(match => match.Games))
        {
            games.Add(game);
            scores.AddRange(game.Scores);
        }

        // Calculate verification counts efficiently in a single pass
        VerificationCounts matchCounts = CalculateVerificationCounts(matches, m => m.VerificationStatus);
        VerificationCounts gameCounts = CalculateVerificationCounts(games, g => g.VerificationStatus);
        VerificationCounts scoreCounts = CalculateVerificationCounts(scores, s => s.VerificationStatus);

        return new TournamentProcessingState
        {
            TournamentId = tournament.Id,
            Abbreviation = tournament.Abbreviation,
            TournamentStatusBefore = beforeStatus,
            TournamentStatusAfter = tournament.VerificationStatus,
            TournamentRejectionReason = tournament.RejectionReason,

            // Total counts
            TotalMatches = matches.Count,
            TotalGames = games.Count,
            TotalScores = scores.Count,

            // Verification counts
            MatchCounts = matchCounts,
            GameCounts = gameCounts,
            ScoreCounts = scoreCounts,

            // Warning counts
            MatchesWithWarnings = matches.Count(m => m.WarningFlags != MatchWarningFlags.None),
            GamesWithWarnings = games.Count(g => g.WarningFlags != GameWarningFlags.None),

            // Rejection groups
            MatchRejections = GroupRejections(matches, m => m.RejectionReason, r => r != MatchRejectionReason.None),
            GameRejections = GroupRejections(games, g => g.RejectionReason, r => r != GameRejectionReason.None),
            ScoreRejections = GroupRejections(scores, s => s.RejectionReason, r => r != ScoreRejectionReason.None)
        };
    }

    /// <summary>
    /// Generates a concise multi-line report limited to 5 lines
    /// </summary>
    /// <param name="state">The processing state</param>
    /// <returns>A formatted multi-line report string (max 5 lines)</returns>
    /// <exception cref="ArgumentNullException">Thrown when state is null</exception>
    public static string GenerateDetailedReport(TournamentProcessingState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var sb = new StringBuilder();

        AppendTournamentLine(sb, state);
        AppendMatchesLine(sb, state);
        AppendGamesLine(sb, state);
        AppendScoresLine(sb, state);
        AppendSummaryLine(sb, state);

        return sb.ToString();
    }

    /// <summary>
    /// Calculates verification counts for a collection of entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="entities">The collection of entities</param>
    /// <param name="statusSelector">Function to select the verification status from an entity</param>
    /// <returns>Verification counts grouped by status</returns>
    private static VerificationCounts CalculateVerificationCounts<T>(
        IEnumerable<T> entities,
        Func<T, VerificationStatus> statusSelector)
    {
        int preVerified = 0;
        int verified = 0;
        int preRejected = 0;
        int rejected = 0;

        foreach (T entity in entities)
        {
            switch (statusSelector(entity))
            {
                case VerificationStatus.PreVerified:
                    preVerified++;
                    break;
                case VerificationStatus.Verified:
                    verified++;
                    break;
                case VerificationStatus.PreRejected:
                    preRejected++;
                    break;
                case VerificationStatus.Rejected:
                    rejected++;
                    break;
                case VerificationStatus.None:
                    break;
            }
        }

        return new VerificationCounts
        {
            PreVerified = preVerified,
            Verified = verified,
            PreRejected = preRejected,
            Rejected = rejected
        };
    }

    /// <summary>
    /// Groups rejections by reason
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TReason">The rejection reason type</typeparam>
    /// <param name="entities">The collection of entities</param>
    /// <param name="reasonSelector">Function to select the rejection reason from an entity</param>
    /// <param name="hasRejection">Function to determine if an entity has a rejection</param>
    /// <returns>List of rejection groups</returns>
    private static List<RejectionGroup> GroupRejections<TEntity, TReason>(
        IEnumerable<TEntity> entities,
        Func<TEntity, TReason> reasonSelector,
        Func<TReason, bool> hasRejection)
        where TReason : notnull
    {
        return entities
            .Select(reasonSelector)
            .Where(hasRejection)
            .GroupBy(r => r.ToString() ?? string.Empty)
            .Select(g => new RejectionGroup
            {
                Reason = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .ToList();
    }

    /// <summary>
    /// Appends the tournament status line to the report
    /// </summary>
    /// <param name="sb">The string builder</param>
    /// <param name="state">The processing state</param>
    private static void AppendTournamentLine(StringBuilder sb, TournamentProcessingState state)
    {
        string tournamentRejection = state.TournamentRejectionReason != TournamentRejectionReason.None
            ? $"{FieldSeparator}Rejection: {state.TournamentRejectionReason}"
            : string.Empty;

        sb.AppendLine($"{TournamentLabel}{ColumnSeparator}{state.Abbreviation} (ID: {state.TournamentId}){FieldSeparator}Status: {state.TournamentStatusBefore} → {state.TournamentStatusAfter}{tournamentRejection}");
    }

    /// <summary>
    /// Appends the matches statistics line to the report
    /// </summary>
    /// <param name="sb">The string builder</param>
    /// <param name="state">The processing state</param>
    private static void AppendMatchesLine(StringBuilder sb, TournamentProcessingState state)
    {
        string info = BuildEntityInfo(state.MatchRejections, state.MatchesWithWarnings);
        sb.AppendLine($"{MatchesLabel}{ColumnSeparator}{ColumnSeparator}Total: {state.TotalMatches}{FieldSeparator}Pre-Ver: {state.MatchCounts.PreVerified}{FieldSeparator}Ver: {state.MatchCounts.Verified}{FieldSeparator}Pre-Rej: {state.MatchCounts.PreRejected}{FieldSeparator}Rej: {state.MatchCounts.Rejected}{info}");
    }

    /// <summary>
    /// Appends the games statistics line to the report
    /// </summary>
    /// <param name="sb">The string builder</param>
    /// <param name="state">The processing state</param>
    private static void AppendGamesLine(StringBuilder sb, TournamentProcessingState state)
    {
        string info = BuildEntityInfo(state.GameRejections, state.GamesWithWarnings);
        sb.AppendLine($"{GamesLabel}{ColumnSeparator}{ColumnSeparator}Total: {state.TotalGames}{FieldSeparator}Pre-Ver: {state.GameCounts.PreVerified}{FieldSeparator}Ver: {state.GameCounts.Verified}{FieldSeparator}Pre-Rej: {state.GameCounts.PreRejected}{FieldSeparator}Rej: {state.GameCounts.Rejected}{info}");
    }

    /// <summary>
    /// Appends the scores statistics line to the report
    /// </summary>
    /// <param name="sb">The string builder</param>
    /// <param name="state">The processing state</param>
    private static void AppendScoresLine(StringBuilder sb, TournamentProcessingState state)
    {
        string info = BuildEntityInfo(state.ScoreRejections, warningCount: 0);
        sb.AppendLine($"{ScoresLabel}{ColumnSeparator}{ColumnSeparator}Total: {state.TotalScores}{FieldSeparator}Pre-Ver: {state.ScoreCounts.PreVerified}{FieldSeparator}Ver: {state.ScoreCounts.Verified}{FieldSeparator}Pre-Rej: {state.ScoreCounts.PreRejected}{FieldSeparator}Rej: {state.ScoreCounts.Rejected}{info}");
    }

    /// <summary>
    /// Appends the summary line to the report
    /// </summary>
    /// <param name="sb">The string builder</param>
    /// <param name="state">The processing state</param>
    private static void AppendSummaryLine(StringBuilder sb, TournamentProcessingState state)
    {
        int totalVerified = state.MatchCounts.Verified + state.GameCounts.Verified + state.ScoreCounts.Verified;
        int totalRejected = state.MatchCounts.Rejected + state.GameCounts.Rejected + state.ScoreCounts.Rejected +
                          state.MatchRejections.Sum(r => r.Count) +
                          state.GameRejections.Sum(r => r.Count) +
                          state.ScoreRejections.Sum(r => r.Count);

        bool automationPassed = state.TournamentStatusAfter is VerificationStatus.Verified or VerificationStatus.PreVerified;
        string result = automationPassed ? "PASSED" : "FAILED";

        sb.AppendLine($"{SummaryLabel}{ColumnSeparator}{ColumnSeparator}Automation Result: {result}{FieldSeparator}Total Verified: {totalVerified}{FieldSeparator}Total Rejected: {totalRejected}");
    }

    /// <summary>
    /// Builds additional info string for entity statistics
    /// </summary>
    /// <param name="rejections">List of rejection groups</param>
    /// <param name="warningCount">Number of entities with warnings</param>
    /// <returns>Formatted info string</returns>
    private static string BuildEntityInfo(List<RejectionGroup> rejections, int warningCount)
    {
        var info = new StringBuilder();

        RejectionGroup? topRejection = rejections.FirstOrDefault();
        if (topRejection is { Count: > 0 })
        {
            info.Append($"{FieldSeparator}Rej: {topRejection.Reason} x{topRejection.Count}");
        }

        if (warningCount > 0)
        {
            info.Append($"{FieldSeparator}Warn: {warningCount}");
        }

        return info.ToString();
    }
}
