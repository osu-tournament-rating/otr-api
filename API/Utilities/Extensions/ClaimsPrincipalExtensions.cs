using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using API.Authorization;

namespace API.Utilities.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Admin"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Admin);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.User"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsUser(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.User);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Client"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsClient(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Client);

    /// <summary>
    /// Denotes the principal as having the <see cref="OtrClaims.Roles.Verifier"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsMatchVerifier(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Verifier);

    /// <summary>
    /// Denotes the principle as having the <see cref="OtrClaims.Roles.Whitelist"/> <see cref="OtrClaims.Role"/>
    /// </summary>
    public static bool IsWhitelisted(this ClaimsPrincipal claimsPrincipal) =>
        IsInRole(claimsPrincipal, OtrClaims.Roles.Whitelist);

    /// <summary>
    /// Gets the custom rate limit of the principle if available
    /// </summary>
    public static int? GetRateLimitOverride(this ClaimsPrincipal claimsPrincipal)
    {
        if (!int.TryParse(claimsPrincipal.FindFirst(OtrClaims.RateLimitOverrides)?.Value, out int limit))
        {
            return null;
        }

        return limit;
    }

    /// <summary>
    /// Gets the <see cref="OtrClaims.Subject"/> of the principle
    /// </summary>
    /// <remarks>
    /// This should only be used when the principal can be assumed to be authenticated. If there is no
    /// expectation of authentication, <see cref="TryGetSubjectId"/> should be used.
    /// <br/>
    /// For example, when used from a controller action annotated with a
    /// <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/>
    /// like <see cref="API.Controllers.MeController.Get"/>, we can be certain that authentication took place
    /// or the body of the action would not be executing. If used in the context of a middleware like the
    /// <see cref="API.Middlewares.WhitelistEnforcementMiddleware"/>, authentication is uncertain and this
    /// method should NOT be used. In this case <see cref="TryGetSubjectId"/> should be used to avoid having to
    /// handle exceptions
    /// </remarks>
    /// <exception cref="ArgumentException">If the principal does not contain a 'sub' claim</exception>
    public static int GetSubjectId(this ClaimsPrincipal claimsPrincipal)
    {
        string? sub = claimsPrincipal.Identity?.Name ?? claimsPrincipal.FindFirst(OtrClaims.Subject)?.Value;
        if (!int.TryParse(sub, out int idInt))
        {
            throw new ArgumentException(
                $"The claims principle did not contain a valid '{OtrClaims.Subject}' claim"
            );
        }

        return idInt;
    }

    /// <summary>
    /// Tries to get the <see cref="OtrClaims.Subject"/> of the principle if available
    /// </summary>
    /// <param name="subjectId">The <see cref="OtrClaims.Subject"/> of the principle, if available</param>
    /// <returns>
    /// True if the <see cref="OtrClaims.Subject"/> of the principle is available, otherwise false
    /// </returns>
    public static bool TryGetSubjectId(
        this ClaimsPrincipal claimsPrincipal,
        [NotNullWhen(true)] out int? subjectId
    )
    {
        subjectId = null;
        try
        {
            subjectId = claimsPrincipal.GetSubjectId();
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the type of <see cref="OtrClaims.Subject"/> described by the principle
    /// </summary>
    /// <returns>
    /// <see cref="OtrClaims.Roles.Client"/> if the subject is a client, otherwise <see cref="OtrClaims.Roles.User"/>
    /// </returns>
    public static string GetSubjectType(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.IsClient() ? OtrClaims.Roles.Client : OtrClaims.Roles.User;

    private static bool IsInRole(ClaimsPrincipal claimsPrincipal, string role)
    {
        return claimsPrincipal.IsInRole(role) || claimsPrincipal.HasClaim(OtrClaims.Role, role);
    }
}
