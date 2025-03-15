namespace Common.Enums.Verification;

/// <summary>
/// The status of a <see cref="Database.Entities.Match"/> in the processing flow
/// </summary>
public enum MatchProcessingStatus
{
    /// <summary>
    /// The <see cref="Database.Entities.Match"/> needs data requested from the osu! API
    /// </summary>
    NeedsData = 0,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 1,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> is awaiting verification from a
    /// <see cref="Database.Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 2,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> needs stat calculation
    /// </summary>
    /// <remarks>Generates the <see cref="Database.Entities.MatchRoster"/> and <see cref="Database.Entities.PlayerMatchStats"/></remarks>
    NeedsStatCalculation = 3,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> is awaiting rating processor data
    /// </summary>
    /// <remarks>Generates all <see cref="Database.Entities.Processor.RatingAdjustment"/>s</remarks>
    NeedsRatingProcessorData = 4,

    /// <summary>
    /// The <see cref="Database.Entities.Match"/> has completed all processing steps
    /// </summary>
    Done = 5
}
