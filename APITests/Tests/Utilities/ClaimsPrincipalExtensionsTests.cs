using System.Security.Claims;
using API.Utilities;

namespace APITests.Tests.Utilities;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void ClaimsPrincipal_Default_HasNoSpecialPermissions()
    {
        var claims = new ClaimsPrincipal();
        Assert.False(claims.IsAdmin());
        Assert.False(claims.IsSystem());
        Assert.False(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsAdmin()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, "admin") }));

        Assert.True(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsMatchVerifier()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, "verifier") }));

        Assert.True(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsUser()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, "user") }));

        Assert.True(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrincipal_IsAdmin_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", "admin") }));

        Assert.True(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsMatchVerifier_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", "verifier") }));

        Assert.True(claims.IsMatchVerifier());
        Assert.False(claims.IsAdmin());
        Assert.False(claims.IsSystem());
    }

    [Fact]
    public void ClaimsPrincipal_IsUser_WhenClaimTypeMapped()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("role", "user") }));

        Assert.True(claims.IsUser());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotAdmin_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", "admin") }));

        Assert.False(claims.IsAdmin());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotMatchVerifier_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", "verifier") }));

        Assert.False(claims.IsMatchVerifier());
    }

    [Fact]
    public void ClaimsPrincipal_IsNotUser_WhenClaimTypeInvalid()
    {
        var claims = new ClaimsPrincipal();
        claims.AddIdentity(new ClaimsIdentity(new List<Claim> { new("123", "user") }));

        Assert.False(claims.IsUser());
    }
}
