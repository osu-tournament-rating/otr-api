using Microsoft.AspNetCore.Authorization;

namespace API.Authorization.Requirements;

/// <summary>
/// Requirement that controls authorization for the <see cref="AuthorizationPolicies.ApiKeyHandler"/> policy
/// </summary>
public class ApiKeyAuthorizationRequirement : IAuthorizationRequirement;
