using Database.Entities;
using Database.Enums.Verification;

namespace TestingUtils.SeededData;

/// <summary>
/// Utility for generating <see cref="Match"/>es with seeded data
/// </summary>
public static class SeededMatch
{
    private static readonly Random s_rand = new();

    /// <summary>
    /// Generates a <see cref="Match"/> with seeded data
    /// </summary>
    /// <remarks>Any properties not given will be randomized</remarks>
    public static Match Generate(
        int? id = null,
        long? osuId = null,
        string? name = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        VerificationStatus? verificationStatus = null,
        MatchRejectionReason? rejectionReason = null,
        MatchProcessingStatus? processingStatus = null,
        Tournament? tournament = null
    )
    {
        Tournament seededTournament = tournament ?? SeededTournament.Generate();

        var seededMatch = new Match
        {
            Id = id ?? s_rand.Next(),
            OsuId = osuId ?? s_rand.NextInt64(),
            Name = name ?? string.Empty,
            VerificationStatus = verificationStatus ?? s_rand.NextEnum<VerificationStatus>(),
            RejectionReason = rejectionReason ?? s_rand.NextEnum<MatchRejectionReason>(),
            ProcessingStatus = processingStatus ?? s_rand.NextEnum<MatchProcessingStatus>(),
            TournamentId = seededTournament.Id,
            Tournament = seededTournament
        };

        if (startTime.HasValue && endTime.HasValue)
        {
            seededMatch.StartTime = startTime.Value;
            seededMatch.EndTime = endTime.Value;
        }

        seededMatch.StartTime = startTime ?? SeededDate.Generate();
        seededMatch.EndTime = endTime ?? SeededDate.GenerateAfter(seededMatch.StartTime);

        seededTournament.Matches.Add(seededMatch);

        return seededMatch;
    }
}
