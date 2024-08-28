using Database.Entities;
using Database.Enums.Verification;

namespace Database.Queries.Extensions;

public static class TournamentQueryExtensions
{
    /// <summary>
    /// Filters a <see cref="VerificationStatus.Verified"/> query for those with a <see cref="VerificationStatus"/>
    /// of <see cref="Tournament"/>
    /// </summary>
    public static IQueryable<Tournament> WhereVerified(this IQueryable<Tournament> query) =>
        query.AsQueryable().Where(x => x.VerificationStatus == VerificationStatus.Verified);

    /// <summary>
    /// Filters a <see cref="Tournament"/> query for those with a <see cref="TournamentProcessingStatus"/>
    /// of <see cref="TournamentProcessingStatus.Done"/>
    /// </summary>
    public static IQueryable<Tournament> WhereProcessingCompleted(this IQueryable<Tournament> query) =>
        query.AsQueryable().Where(x => x.ProcessingStatus == TournamentProcessingStatus.Done);
}
