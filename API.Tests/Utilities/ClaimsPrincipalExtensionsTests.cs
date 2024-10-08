using System.Security.Claims;
using API.Authorization;
using API.Utilities.Extensions;
using Database.Entities;

namespace APITests.Utilities;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void ClaimsPrincipal_Default_HasNoSpecialPermissions()
    {
        var claims = new ClaimsPrincipal();
        Assert.False(claims.IsAdmin());
        Assert.False(claims.IsMatchVerifier());
        Assert.False(claims.IsWhitelisted());
    }

    [Fact]
    public void ClaimsPrincipal_IsAdmin()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.Role, OtrClaims.Roles.Admin) }));

        Assert.True(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsMatchVerifier()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.Role, OtrClaims.Roles.Verifier) }));

        Assert.True(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsUser()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.Role, OtrClaims.Roles.User) }));

        Assert.True(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrincipal_IsClient()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.Role, OtrClaims.Roles.Client) }));

        Assert.True(claims.IsClient());
    }

    [Fact]
    public void ClaimsPrinciple_IsWhitelisted()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.Role, OtrClaims.Roles.Whitelist) }));

        Assert.True(claims.IsWhitelisted());
    }

    [Fact]
    public void ClaimsPrincipal_GetTokenType()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(OtrClaims.TokenType, OtrClaims.TokenTypes.AccessToken) }));

        Assert.True(claims.GetTokenType() is OtrClaims.TokenTypes.AccessToken);
    }

    [Fact]
    public void ClaimsPrincipal_GetRateLimitOverrides()
    {
        var claims = new ClaimsPrincipal();
        var overrides = new RateLimitOverrides { PermitLimit = 100, Window = 20 };
        var serializedOverrides = RateLimitOverridesSerializer.Serialize(overrides);

        claims.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new(OtrClaims.RateLimitOverrides, serializedOverrides)
        }));

        Assert.NotNull(claims.GetRateLimitOverrides());
        Assert.Equivalent(overrides, claims.GetRateLimitOverrides());
    }
}
