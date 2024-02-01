using API.Utilities;

namespace APITests.Utilities;

public class StringExtensionsTests
{
	[Theory]
	[InlineData("Emerald_Ages", "Emerald Ages")]
	[InlineData("emerald ages", "emerald ages")]
	[InlineData("cookiezi", "cookiezi")]
	[InlineData("M_I_L_E_S", "M I L E S")]
	public void ReplaceUnderscores_TransformsToSpaces(string input, string expected)
	{
		// Arrange

		// Act
		string result = input.ReplaceUnderscores();

		// Assert
		Assert.Equal(result, expected);
	}
}