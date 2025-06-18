using System.Diagnostics.CodeAnalysis;
using API.Utilities.DataAnnotations;

namespace APITests.Utilities.DataAnnotations;

[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class BitLengthAttributeTests
{
    [Theory]
    [InlineData("A", 8, 8, true)] // "A" exactly 8 bits
    [InlineData("1", 8, 8, true)] // "1" exactly 8 bits
    [InlineData("AB", 16, 16, true)] // "AB" exactly 16 bits
    [InlineData("ABCD", 0, 31, false)] // "ABCD" greater than 31 bits
    [InlineData("ABCD", 33, 36, false)] // "ABCD" less than 33 bits
    [InlineData("ABCDEFGHIJKLMNOP", 128, 256, true)] // "ABCDEFGHIJKLMNOP" at least 128 bits
    [InlineData("ABC", 0, 16, false)] // "ABC" greater than 16 bits
    [InlineData("", 0, 8, true)] // string.Empty considered valid
    [InlineData(null, 0, 8, true)] // null considered valid
    public void Length_IsValid(string? input, int min, int max, bool expected)
    {
        bool isValid = new BitLengthAttribute { Minimum = min, Maximum = max }.IsValid(input);

        Assert.Equal(expected, isValid);
    }
}
