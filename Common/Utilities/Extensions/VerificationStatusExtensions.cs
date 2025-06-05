using Common.Enums.Verification;

namespace Common.Utilities.Extensions;

public static class VerificationStatusExtensions
{
    /// <summary>
    /// Checks whether the <see cref="VerificationStatus"/> is <see cref="VerificationStatus.PreVerified"/>/<see cref="VerificationStatus.Verified"/> or not
    /// </summary>
    public static bool IsPreVerifiedOrVerified(this VerificationStatus status) =>
        status is VerificationStatus.PreVerified or VerificationStatus.Verified;

    /// <summary>
    /// Confirms a pre-verification status by converting PreRejected to Rejected and PreVerified to Verified
    /// </summary>
    /// <param name="status">The verification status to confirm</param>
    /// <returns>The confirmed status, or the original status if it's not a pre-status</returns>
    public static VerificationStatus ConfirmPreStatus(this VerificationStatus status) =>
        status switch
        {
            VerificationStatus.PreRejected => VerificationStatus.Rejected,
            VerificationStatus.PreVerified => VerificationStatus.Verified,
            _ => status
        };
}
