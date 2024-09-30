using API.Authorization;
using API.Utilities;

namespace APITests.Utilities;

public class OtrClaimsTests
{
    [Theory]
    [InlineData(OtrClaims.Admin, true)]
    [InlineData(OtrClaims.Verifier, true)]
    [InlineData(OtrClaims.Submitter, true)]
    [InlineData(OtrClaims.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData(OtrClaims.User, false)]
    [InlineData(OtrClaims.Client, false)]
    [InlineData("Admin", false)]
    [InlineData("Garbage", false)]
    public void IsUserAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.IsUserAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrClaims.System, true)]
    [InlineData(OtrClaims.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData(OtrClaims.User, false)]
    [InlineData(OtrClaims.Client, false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsClientAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.IsClientAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrClaims.User, true)]
    [InlineData(OtrClaims.Client, true)]
    [InlineData(OtrClaims.System, true)]
    [InlineData(OtrClaims.Admin, true)]
    [InlineData(OtrClaims.Verifier, true)]
    [InlineData(OtrClaims.Submitter, true)]
    [InlineData(OtrClaims.Whitelist, true)]
    [InlineData(OtrClaims.RateLimitOverrides, true)]
    [InlineData("User", false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsValidClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrClaims.IsValidRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }
}
