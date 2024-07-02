namespace Database.Enums.Verification;

/// <summary>
/// The status of a match in the processing flow
/// </summary>
public enum MatchProcessingStatus
{
    /// <summary>
    /// Needs data requested from the osu! API
    /// </summary>
    NeedsData = 0,

    /// <summary>
    /// Data has been retrieved from the osu! API and parsed, ready for automation checks
    /// </summary>
    NeedsAutomationChecks = 1,

    /// <summary>
    /// Automation checks have completed, ready for verification
    /// </summary>
    NeedsVerification = 2,

    /// <summary>
    /// TBD
    /// </summary>
    NeedsStatCalculation = 3,

    /// <summary>
    /// All steps have been completed, match is ready for human review
    /// </summary>
    Done = 4
}
