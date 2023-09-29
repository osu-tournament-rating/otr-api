using API.Osu;

namespace APITests.Osu;

public class LobbyNameCheckerTests
{
	[Test]
	[TestCase("slot: (хохлорусы) vs (Mr.Worldwide)", "slot")]
	[TestCase("megacup: (Nałęczowianka) vs (ff we have axolotl)", "megacup")]
	[TestCase("PDT: (Пиздатые Плюшки) vs (Взрывные @everyone котюнчики)", "PDT")]
	[TestCase("CFC: (mp close) vs (フフフ)", "CFC")]
	[TestCase("BotB: (--... ..--- --...) vs (:monkey: but emoji)", "BotB")]
	[TestCase("CAT18: 1k-9k L12 |(Minami-Kotori) vs (KomachiBaka)", "CAT18")]
	[TestCase("\"6 digit cup: (group\"\"3\"\") vs (group\"\"3\"\")\"", "6 digit cup")]
	[TestCase("AHSESPORTS: (The S.S.) vs (over salted utakus)", "AHSESPORTS")]
	[TestCase("BGCC2: Водоземската Мафия vs po4ina", "BGCC2")]
	[TestCase("Some Tournament: (ABC) vs (abc)", "Some Tournament")]
	[TestCase("BGCC2: Водоземската Мафия vs. po4ina", "BGCC2")]
	[TestCase("BGCC2: Водоземската Мафия VS. po4ina", "BGCC2")]
	[TestCase("BGCC2: Водоземската Мафия VS po4ina", "BGCC2")]
	[TestCase("CC: (8mi8) vs (-- Waffle --)", "CC")]
	[TestCase("CC: (()()()) vs ((((((((()))", "CC")]
	[TestCase("CC: ()) vs. (()", "CC")]
	[TestCase("CC: (() vs ())", "CC")]
	public void Patterns_Regex_ShouldMatch(string name, string abbreviation) => Assert.That(LobbyNameChecker.IsNameValid(name, abbreviation), Is.True);

	[Test]
	[TestCase("test (a) vs. (b)", "test")]
	[TestCase("test (a) vs (b)", "test")]
	[TestCase("WDTWE (:smirk_cat:) vs (RAMSING)", "WDTWE")]
	[TestCase("WDTWE :(smirk_cat:) vs (RAMSING", "WDTWE")]
	[TestCase("WDTWE :smirk_cat: vs RAMSING", "WDTWE")]
	[TestCase("CC : (8mi8) vs (-- Waffle --)", "CC")]
	public void Patterns_Regex_ShouldNotMatch(string name, string abbreviation) => Assert.That(LobbyNameChecker.IsNameValid(name, abbreviation), Is.False);
}