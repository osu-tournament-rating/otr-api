using API.Utilities.DataAnnotations;

namespace APITests.Utilities.DataAnnotations;

public class PositiveAttributeTests
{
    public static TheoryData<int, bool> TestData() =>
        new()
        {
            { int.MinValue, false },
            { -100, false },
            { -1, false },
            { 1, true },
            { 100, true },
            { int.MaxValue, true }
        };

    [Theory]
    [MemberData(nameof(TestData))]
    [InlineData(0, false)]
    public void PositiveValue_IsValid(int value, bool expected)
    {
        // Arrange
        var attribute = new PositiveAttribute();

        // Act
        var isValid = attribute.IsValid(value);

        // Assert
        Assert.Equal(expected, isValid);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    [InlineData(0, true)]
    public void WhenAllowZero_Zero_IsValid(int value, bool expected)
    {
        // Arrange
        var attribute = new PositiveAttribute(allowZero: true);

        // Act
        var isValid = attribute.IsValid(value);

        // Assert
        Assert.Equal(expected, isValid);
    }
}
