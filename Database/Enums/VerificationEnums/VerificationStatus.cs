namespace Database.Enums.VerificationEnums;

/// <summary>
/// The verification status of a <see cref="Entities.Match"/> or <see cref="Entities.Game"/>
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// Has not been assigned a verification status,
    /// is awaiting processing
    /// </summary>
    None = 0,
    /// <summary>
    /// Ready for human review, does not pass all automation checks
    /// </summary>
    PreRejected = 1,
    /// <summary>
    /// Ready for human review, passes all automation checks
    /// </summary>
    PreVerified = 2,
    /// <summary>
    /// Determined via human review or automation checks to be invalid
    /// </summary>
    /// <remarks>
    /// Some automation checks will result in a concrete rejection upon failing them
    /// </remarks>
    Rejected = 3,
    /// <summary>
    /// Passed manual review. Data is fit for rating calculations
    /// </summary>
    Verified = 4
}
