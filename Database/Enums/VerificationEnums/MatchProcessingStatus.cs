namespace Database.Enums.VerificationEnums;

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
    /// Data has been retrieved from the osu! API and parsed,
    /// ready for automation checks
    /// </summary>
    NeedsAutomationChecks = 1,
    /// <summary>
    /// All steps have been completed, match is ready for human review
    /// </summary>
    Done = 2
}
