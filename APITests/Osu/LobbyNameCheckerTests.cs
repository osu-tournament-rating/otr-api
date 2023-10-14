using API.Osu;

namespace APITests.Osu;

public class LobbyNameCheckerTests
{
	[Theory]
	[InlineData("slot: (хохлорусы) vs (Mr.Worldwide)", "slot")]
	[InlineData("megacup: (Nałęczowianka) vs (ff we have axolotl)", "megacup")]
	[InlineData("PDT: (Пиздатые Плюшки) vs (Взрывные @everyone котюнчики)", "PDT")]
	[InlineData("CFC: (mp close) vs (フフフ)", "CFC")]
	[InlineData("BotB: (--... ..--- --...) vs (:monkey: but emoji)", "BotB")]
	[InlineData("CAT18: 1k-9k L12 |(Minami-Kotori) vs (KomachiBaka)", "CAT18")]
	[InlineData("AHSESPORTS: (The S.S.) vs (over salted utakus)", "AHSESPORTS")]
	[InlineData("Some Tournament: (ABC) vs (abc)", "Some Tournament")]
	[InlineData("CC: (8mi8) vs (-- Waffle --)", "CC")]
	[InlineData("CC: (()()()) vs ((((((((()))", "CC")]
	[InlineData("CC: ()) vs. (()", "CC")]
	[InlineData("CC: (() vs ())", "CC")]
	[InlineData("test (a) vs. (b)", "test")]
	[InlineData("test (a) vs (b)", "test")]
	[InlineData("WDTWE (:smirk_cat:) vs (RAMSING)", "WDTWE")]
	[InlineData("CC : (8mi8) vs (-- Waffle --)", "CC")]
	[InlineData("6 digit cup: (group\"\"3\"\") vs (group\"\"3\"\")\"", "6 digit cup")]
	public void Patterns_Regex_ShouldMatch(string name, string abbreviation) => Assert.True(LobbyNameChecker.IsNameValid(name, abbreviation));

	[Theory]
	[InlineData("THIS SHOULD FAIL", "THIS SHOULD FAIL")] // Tests for teams existing (e.g. "(a) vs (b)")
	[InlineData("THIS: SHOULD FAIL", "THIS")]
	[InlineData("(a) vs. (b)", "SomeAbbreviationThat'sMissing")]
	[InlineData("Test: A vs. (b)", "Test")]
	[InlineData("Test: (A) vs. b", "Test")]
	[InlineData("BGCC2: Водоземската Мафия vs. po4ina", "BGCC2")]
	[InlineData("BGCC2: Водоземската Мафия VS. po4ina", "BGCC2")]
	[InlineData("BGCC2: Водоземската Мафия VS po4ina", "BGCC2")]
	[InlineData("\"6 digit cup: (group\"\"3\"\") vs (group\"\"3\"\")\"", "6 digit cup")]
	[InlineData("BGCC2: Водоземската Мафия vs po4ina", "BGCC2")]
	[InlineData("WDTWE :smirk_cat: vs RAMSING", "WDTWE")]
	[InlineData("WDTWE :(smirk_cat:) vs (RAMSING", "WDTWE")]
	public void Patterns_Regex_ShouldNotMatch(string name, string abbreviation) => Assert.False(LobbyNameChecker.IsNameValid(name, abbreviation));
}