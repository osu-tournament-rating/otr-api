namespace API.Authorization;

/// <summary>
/// String constants that represent authorization policies
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Policy that allows access from the user that owns the resource as well
    /// as any <see cref="OtrClaims.Roles.Admin"/> users
    /// </summary>
    public const string AccessUserResources = "AccessUserResources";

    /// <summary>
    /// Policy that allows the API to determine if the request is coming from a trusted
    /// source, such as otr-web. This is effectively the same as [AllowAnonymous], except only
    /// for requests coming from trusted sources.
    /// </summary>
    public const string ApiKeyAuthorization = "ApiKey";

    /// <summary>
    /// Policy that controls access based on whether or not the whitelist is enabled
    /// </summary>
    public const string Whitelist = "Whitelist";

    /// <summary>
    /// Collection of all <see cref="AuthorizationPolicies"/>
    /// </summary>
    public static readonly string[] Policies =
    [
        AccessUserResources
    ];

    /// <summary>
    /// Gets the description of an authorization policy
    /// </summary>
    public static string GetDescription(string policy) =>
        policy switch
        {
            AccessUserResources =>
                "Policy that allows access from the user that owns the resource as well as any admin users",
            _ => "No description available."
        };
}
