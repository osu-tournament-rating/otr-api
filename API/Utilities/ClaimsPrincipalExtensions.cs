using System.Security.Claims;

namespace API.Utilities;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsInRole("Admin") || IsSystem(claimsPrincipal);

    /// <summary>
    /// Indicates whether the user is a system user.
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static bool IsSystem(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.IsInRole("System");

    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsInRole("MatchVerifier") || IsAdmin(claimsPrincipal) || IsSystem(claimsPrincipal);
}
