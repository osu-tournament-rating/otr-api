namespace Database.Enums.Verification;

/// <summary>
/// The status of a <see cref="Entities.Match"/> in the processing flow
/// </summary>
public enum MatchProcessingStatus
{
    /// <summary>
    /// The <see cref="Entities.Match"/> needs data requested from the osu! API
    /// </summary>
    NeedsData = 0,

    /// <summary>
    /// The <see cref="Entities.Match"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 1,

    /// <summary>
    /// The <see cref="Entities.Match"/> is awaiting verification from a
    /// <see cref="Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 2,

    /// <summary>
    /// The <see cref="Entities.Match"/> needs stat calculation
    /// </summary>
    /// <remarks>Generates the <see cref="Entities.MatchWinRecord"/> and <see cref="Entities.PlayerMatchStats"/></remarks>
    NeedsStatCalculation = 3,

    /// <summary>
    /// The <see cref="Entities.Match"/> is awaiting rating processor data
    /// </summary>
    /// <remarks>Generates all <see cref="Entities.Processor.RatingAdjustment"/>s</remarks>
    NeedsRatingProcessorData = 4,

    /// <summary>
    /// The <see cref="Entities.Match"/> has completed all processing steps
    /// </summary>
    Done = 5
}
