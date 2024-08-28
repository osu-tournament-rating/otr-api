namespace Database.Enums.Verification;

/// <summary>
/// The status of a <see cref="Entities.GameScore"/> in the processing flow
/// </summary>
public enum ScoreProcessingStatus
{
    /// <summary>
    /// The <see cref="Entities.GameScore"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 0,

    /// <summary>
    /// The <see cref="Entities.GameScore"/> is awaiting verification from a
    /// <see cref="Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 1,

    /// <summary>
    /// The <see cref="Entities.GameScore"/> has completed all processing steps
    /// </summary>
    Done = 2
}
