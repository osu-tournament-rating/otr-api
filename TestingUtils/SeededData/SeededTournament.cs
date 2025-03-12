using Common.Enums.Enums;
using Common.Enums.Enums.Verification;
using Database.Entities;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Tournament"/>s with seeded data
/// </summary>
public static class SeededTournament
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Tournament"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Tournament Generate(
        int? id = null,
        string? name = null,
        string? abbreviation = null,
        string? forumUrl = null,
        int? rankRangeLowerBound = null,
        Ruleset? ruleset = null,
        int? teamSize = null,
        VerificationStatus? verificationStatus = null,
        TournamentRejectionReason? rejectionReason = null,
        TournamentProcessingStatus? processingStatus = null
    ) =>
        new()
        {
            Id = id ?? s_rand.Next(),
            Name = name ?? string.Empty,
            Abbreviation = abbreviation ?? string.Empty,
            ForumUrl = forumUrl ?? string.Empty,
            RankRangeLowerBound = rankRangeLowerBound ?? s_rand.NextInclusive(1_000_000),
            Ruleset = ruleset ?? s_rand.NextEnum<Ruleset>(),
            TeamLobbySize = teamSize ?? s_rand.NextInclusive(1, 8),
            VerificationStatus = verificationStatus ?? s_rand.NextEnum<VerificationStatus>(),
            RejectionReason = rejectionReason ?? s_rand.NextEnum<TournamentRejectionReason>(),
            ProcessingStatus = processingStatus ?? s_rand.NextEnum<TournamentProcessingStatus>()
        };
}
