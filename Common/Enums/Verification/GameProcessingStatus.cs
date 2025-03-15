namespace Common.Enums.Verification;

/// <summary>
/// The status of a <see cref="Database.Entities.Game"/> in the processing flow
/// </summary>
public enum GameProcessingStatus
{
    /// <summary>
    /// The <see cref="Database.Entities.Game"/> needs automation checks
    /// </summary>
    NeedsAutomationChecks = 0,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/> is awaiting verification from a
    /// <see cref="Database.Entities.User"/> with verifier permission
    /// </summary>
    NeedsVerification = 1,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/> needs stat calculation
    /// </summary>
    /// <remarks>Generates the <see cref="Database.Entities.GameRoster"/></remarks>
    NeedsStatCalculation = 2,

    /// <summary>
    /// The <see cref="Database.Entities.Game"/> has completed all processing steps
    /// </summary>
    Done = 3
}
