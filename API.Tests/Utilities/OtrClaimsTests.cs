using API.Authorization;
using API.Utilities;

namespace APITests.Utilities;

public class OtrClaimsTests
{
    [Theory]
    [InlineData(OtrClaims.Roles.Admin, true)]
    [InlineData(OtrClaims.Roles.Verifier, true)]
    [InlineData(OtrClaims.Roles.Submitter, true)]
    [InlineData(OtrClaims.Roles.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData(OtrClaims.Roles.User, false)]
    [InlineData(OtrClaims.Roles.Client, false)]
    [InlineData("Admin", false)]
    [InlineData("Garbage", false)]
    public void IsUserAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.Roles.IsUserAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrClaims.Roles.System, true)]
    [InlineData(OtrClaims.Roles.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData(OtrClaims.Roles.User, false)]
    [InlineData(OtrClaims.Roles.Client, false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsClientAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.Roles.IsClientAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrClaims.Roles.User, true)]
    [InlineData(OtrClaims.Roles.Client, true)]
    [InlineData(OtrClaims.Roles.System, true)]
    [InlineData(OtrClaims.Roles.Admin, true)]
    [InlineData(OtrClaims.Roles.Verifier, true)]
    [InlineData(OtrClaims.Roles.Submitter, true)]
    [InlineData(OtrClaims.Roles.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData("User", false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsValidClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.Roles.IsValidRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }
}
