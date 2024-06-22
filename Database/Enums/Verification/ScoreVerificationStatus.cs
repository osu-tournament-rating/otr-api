namespace Database.Enums.Verification;

/// <summary>
/// The status of a score in the verification process
/// </summary>
public enum ScoreVerificationStatus
{
    /// <summary>
    /// Score has not completed automated checks
    /// </summary>
    None,

    /// <summary>
    /// Score is determined to be invalid by automation checks or manual review
    /// </summary>
    Rejected,

    /// <summary>
    /// Score is determined to be valid by automation checks or manual review
    /// </summary>
    Verified
}
