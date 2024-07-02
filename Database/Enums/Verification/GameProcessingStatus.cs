namespace Database.Enums.Verification;

/// <summary>
/// The status of a <see cref="Entities.Game"/> in the processing flow
/// </summary>
public enum GameProcessingStatus
{
    /// <summary>
    /// The <see cref="Entities.Game"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 0,

    /// <summary>
    /// The <see cref="Entities.Game"/> is awaiting verification from a
    /// <see cref="Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 1,

    /// <summary>
    /// The <see cref="Entities.Game"/> needs stat calculation
    /// </summary>
    /// <remarks>Generates the <see cref="Entities.GameWinRecord"/></remarks>
    NeedsStatCalculation = 2,

    /// <summary>
    /// The <see cref="Entities.Game"/> has completed all processing steps
    /// </summary>
    Done = 3
}
