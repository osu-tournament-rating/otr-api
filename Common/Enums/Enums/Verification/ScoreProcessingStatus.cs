namespace Common.Enums.Enums.Verification;

/// <summary>
/// The status of a <see cref="Database.Entities.GameScore"/> in the processing flow
/// </summary>
public enum ScoreProcessingStatus
{
    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 0,

    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/> is awaiting verification from a
    /// <see cref="Database.Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 1,

    /// <summary>
    /// The <see cref="Database.Entities.GameScore"/> has completed all processing steps
    /// </summary>
    Done = 2
}
