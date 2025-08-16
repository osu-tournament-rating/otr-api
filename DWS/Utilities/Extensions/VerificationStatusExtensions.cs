using Common.Enums.Verification;

namespace DWS.Utilities.Extensions;

public static class VerificationStatusExtensions
{
    public static bool IsPreVerifiedOrVerified(this VerificationStatus status) =>
        status is VerificationStatus.PreVerified or VerificationStatus.Verified;
}
