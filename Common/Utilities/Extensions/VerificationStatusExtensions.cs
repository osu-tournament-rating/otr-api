using Common.Enums.Verification;

namespace Common.Utilities.Extensions;

public static class VerificationStatusExtensions
{
    /// <summary>
    /// Checks whether the <see cref="VerificationStatus"/> is <see cref="VerificationStatus.PreVerified"/>/<see cref="VerificationStatus.Verified"/> or not
    /// </summary>
    public static bool IsPreVerifiedOrVerified(this VerificationStatus status) =>
        status is VerificationStatus.PreVerified or VerificationStatus.Verified;
}
