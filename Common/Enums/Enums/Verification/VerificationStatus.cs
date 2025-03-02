namespace Common.Enums.Enums.Verification;

/// <summary>
/// The verification status of a <see cref="Database.Entities.Tournament"/>,
/// <see cref="Database.Entities.Match"/>, <see cref="Database.Entities.Game"/>, or <see cref="Database.Entities.GameScore"/>
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// Verification status has not yet been assigned
    /// </summary>
    None = 0,

    /// <summary>
    /// The Data Worker has identified an issue during processing
    /// </summary>
    PreRejected = 1,

    /// <summary>
    /// The Data Worker has not identified any issues during processing
    /// </summary>
    PreVerified = 2,

    /// <summary>
    /// Determined to be unfit for ratings by manual review
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Determined to be fit for ratings by manual review
    /// </summary>
    Verified = 4
}
