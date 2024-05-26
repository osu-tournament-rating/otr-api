namespace Database.Enums.VerificationEnums;

/// <summary>
/// The status of a tournament in the processing flow
/// </summary>
public enum TournamentProcessingStatus
{
    /// <summary>
    /// The tournament needs automation checks
    /// </summary>
    NeedsAutomationChecks = 0,
    /// <summary>
    /// The tournament has completed automation checks
    /// </summary>
    Done = 1
}
