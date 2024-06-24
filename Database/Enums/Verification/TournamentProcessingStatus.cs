namespace Database.Enums.Verification;

/// <summary>
/// The status of a tournament in the processing flow
/// </summary>
public enum TournamentProcessingStatus
{
    /// <summary>
    /// The tournament is awaiting approval from a verifier before data is gathered
    /// </summary>
    AwaitingApproval = 0,

    /// <summary>
    /// The tournament needs automation checks
    /// </summary>
    NeedsAutomationChecks = 1,

    /// <summary>
    /// The tournament has completed automation checks
    /// </summary>
    Done = 2
}
