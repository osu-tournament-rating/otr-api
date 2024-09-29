using API.Authorization;
using API.Utilities;

namespace APITests.Utilities;

public class OtrJwtRolesTests
{
    [Theory]
    [InlineData(OtrJwtRoles.Admin, true)]
    [InlineData(OtrJwtRoles.Verifier, true)]
    [InlineData(OtrJwtRoles.Submitter, true)]
    [InlineData(OtrJwtRoles.Whitelist, true)]
    [InlineData(OtrJwtRoles.RateLimitOverrides, true)]
    [InlineData(OtrJwtRoles.User, false)]
    [InlineData(OtrJwtRoles.Client, false)]
    [InlineData("Admin", false)]
    [InlineData("Garbage", false)]
    public void IsUserAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrJwtRoles.IsUserAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrJwtRoles.System, true)]
    [InlineData(OtrJwtRoles.Whitelist, true)]
    [InlineData(OtrJwtRoles.RateLimitOverrides, true)]
    [InlineData(OtrJwtRoles.User, false)]
    [InlineData(OtrJwtRoles.Client, false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsClientAssignableClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrJwtRoles.IsClientAssignableRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OtrJwtRoles.User, true)]
    [InlineData(OtrJwtRoles.Client, true)]
    [InlineData(OtrJwtRoles.System, true)]
    [InlineData(OtrJwtRoles.Admin, true)]
    [InlineData(OtrJwtRoles.Verifier, true)]
    [InlineData(OtrJwtRoles.Submitter, true)]
    [InlineData(OtrJwtRoles.Whitelist, true)]
    [InlineData(OtrJwtRoles.RateLimitOverrides, true)]
    [InlineData("User", false)]
    [InlineData("System", false)]
    [InlineData("Garbage", false)]
    public void IsValidClaim_ReturnsCorrectBool(string claim, bool expected)
    {
        // Arrange

        // Act
        var actual = OtrJwtRoles.IsValidRole(claim);

        // Assert
        Assert.Equal(expected, actual);
    }
}
