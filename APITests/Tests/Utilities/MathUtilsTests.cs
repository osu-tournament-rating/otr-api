using API.Utilities;

namespace APITests.Tests.Utilities;

public class MathUtilsTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 2, 1)]
    [InlineData(2, 1, 2, 3)]
    [InlineData(2, 3, 2, 1)]
    [InlineData(5, 0, 0, 5, 5, 5)]
    public void Median_IsValid(int result, params int[] values)
    {
        var list = values.Select(x => (int?)x).ToList();
        Assert.Equal(result, MathUtils.Median(list));
    }
}
