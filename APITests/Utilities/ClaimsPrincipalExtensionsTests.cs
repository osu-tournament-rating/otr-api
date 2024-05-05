using System.Security.Claims;
using API.Utilities;

namespace APITests.Utilities;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void ClaimsPrincipal_Default_HasNoSpecialPermissions()
    {
        var claims = new ClaimsPrincipal();
        Assert.False(claims.IsAdmin());
        Assert.False(claims.IsSystem());
        Assert.False(claims.IsMatchVerifier());
        Assert.False(claims.IsWhitelisted());
    }

    [Fact]
    public void ClaimsPrincipal_IsAdmin()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, OtrClaims.Admin) }));

        Assert.True(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsMatchVerifier()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, OtrClaims.Verifier) }));

        Assert.True(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsUser()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, OtrClaims.User) }));

        Assert.True(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrinciple_IsWhitelisted()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, OtrClaims.Whitelist) }));

        Assert.True(claims.IsWhitelisted());
    }

    [Fact]
    public void ClaimsPrincipal_IsAdmin_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", OtrClaims.Admin) }));

        Assert.True(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsMatchVerifier_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", OtrClaims.Verifier) }));

        Assert.True(claims.IsMatchVerifier());
        Assert.False(claims.IsAdmin());
        Assert.False(claims.IsSystem());
    }

    [Fact]
    public void ClaimsPrincipal_IsUser_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", OtrClaims.User) }));

        Assert.True(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrinciple_IsWhitelisted_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", OtrClaims.Whitelist) }));

        Assert.True(claims.IsWhitelisted());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotAdmin_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", OtrClaims.Admin) }));

        Assert.False(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotMatchVerifier_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", OtrClaims.Verifier) }));

        Assert.False(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotUser_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", OtrClaims.User) }));

        Assert.False(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrinciple_IsNotWhitelisted_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", OtrClaims.Whitelist) }));

        Assert.False(claims.IsWhitelisted());
    }
}
