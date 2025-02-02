using API.Utilities.DataAnnotations;

namespace APITests.Utilities.DataAnnotations;

public class PercentageAttributeTests
{
    [Theory]
    [InlineData(100, false)]
    [InlineData(25, false)]
    [InlineData(-2, false)]
    [InlineData(-0.5, false)]
    [InlineData(1.000001, false)]
    [InlineData(1.00, true)]
    [InlineData(0.25, true)]
    [InlineData(0.52525, true)]
    [InlineData(0.00, true)]
    public void Percentage_IsValid(double percentage, bool expected)
    {
        // Arrange
        var attribute = new PercentageAttribute();

        // Act
        var isValid = attribute.IsValid(percentage);

        // Assert
        Assert.Equal(expected, isValid);
    }
}
