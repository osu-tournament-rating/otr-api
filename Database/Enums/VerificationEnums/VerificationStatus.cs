namespace Database.Enums.VerificationEnums;

/// <summary>
/// The verification status of a <see cref="Entities.Tournament"/>,
/// <see cref="Entities.Match"/>, or <see cref="Entities.Game"/>
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// Has not been assigned a verification status,
    /// is awaiting processing
    /// </summary>
    None = 0,
    /// <summary>
    /// Does not pass all automation checks, ready for human review
    /// </summary>
    PreRejected = 1,
    /// <summary>
    /// Passes all automation checks, ready for human review
    /// </summary>
    PreVerified = 2,
    /// <summary>
    /// Determined to be invalid by automation checks or manual review
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
