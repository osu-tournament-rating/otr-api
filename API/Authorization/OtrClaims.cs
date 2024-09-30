namespace API.Authorization;

/// <summary>
/// String values that represent valid roles for users and clients that serve as an access control
/// system for resource routes
/// </summary>
public static class OtrClaims
{
    /// <summary>
    /// Role for all users
    /// </summary>
    public const string User = "user";

    /// <summary>
    /// Role for all clients
    /// </summary>
    public const string Client = "client";

    /// <summary>
    /// Role for internal privileged clients
    /// </summary>
    /// <example>o!TR processor</example>
    public const string System = "system";

    /// <summary>
    /// Role for privileged users
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// Role for users with permission to verify matches
    /// </summary>
    public const string Verifier = "verifier";

    // TODO: Convert this to work in the inverse
    // Instead of granting all users "submit", we can grant restricted users "restricted" which would
    // flag the submission flow to check the user for potential submission restriction
    /// <summary>
    /// Role for users with permission to submit matches
    /// </summary>
    public const string Submitter = "submit";

    /// <summary>
    /// Role for users and clients to restrict api access
    /// </summary>
    public const string Whitelist = "whitelist";

    /// <summary>
    /// Claim granted to users or clients to denote overrides to the default rate limit
    /// </summary>
    public const string RateLimitOverrides = "ratelimitoverrides";

    /// <summary>
    /// Denotes the given role is assignable to a user
    /// </summary>
    public static bool IsUserAssignableRole(string role)
    {
        return role switch
        {
            // 'User' not included because we only encode that role to the JWT
            Admin => true,
            Verifier => true,
            Submitter => true,
            Whitelist => true,
            RateLimitOverrides => true,
            _ => false
        };
    }

    /// <summary>
    /// Denotes the given role is assignable to a client
    /// </summary>
    public static bool IsClientAssignableRole(string role)
    {
        return role switch
        {
            // 'Client' not included because we only encode that role to the JWT
            System => true,
            Whitelist => true,
            RateLimitOverrides => true,
            _ => false
        };
    }

    /// <summary>
    /// Denotes the given role is valid
    /// </summary>
    public static bool IsValidRole(string role)
    {
        return role switch
        {
            User => true,
            Client => true,
            System => true,
            Admin => true,
            Verifier => true,
            Submitter => true,
            Whitelist => true,
            RateLimitOverrides => true,
            _ => false
        };
    }
}
