using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Requirements;

/// <summary>
/// Requirement that controls user access for the policy <see cref="AuthorizationPolicies.AccessUserResources"/>
/// </summary>
public class AccessUserResourcesRequirement(bool allowSelf) : IAuthorizationRequirement
{
    /// <summary>
    /// Denotes users as having access to the target resource
    /// </summary>
    public bool AllowSelf { get; } = allowSelf;
}
