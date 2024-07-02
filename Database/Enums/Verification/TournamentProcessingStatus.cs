namespace Database.Enums.Verification;

/// <summary>
/// The status of a tournament in the processing flow
/// </summary>
public enum TournamentProcessingStatus
{
    /// <summary>
    /// The tournament is awaiting approval from a verifier before data is gathered
    /// </summary>
    NeedsApproval = 0,

    /// <summary>
    /// The tournament is awaiting match data population
    /// </summary>
    NeedsData = 1,

    /// <summary>
    /// The tournament needs automation checks
    /// </summary>
    NeedsAutomationChecks = 2,

    /// <summary>
    /// The tournament is awaiting verification from a verifier
    /// </summary>
    NeedsVerification = 3,

    /// <summary>
    /// The tournament has completed automation checks
    /// </summary>
    Done = 4
}
