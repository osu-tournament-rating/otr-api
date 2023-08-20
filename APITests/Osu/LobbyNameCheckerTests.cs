using API.Osu;

namespace APITests.Osu;

public class LobbyNameCheckerTests
{
	/**
	 * 
	 */
	[Test]
	[TestCase("slot: (хохлорусы) vs (Mr.Worldwide)")]
	[TestCase("megacup: (Nałęczowianka) vs (ff we have axolotl)")]
	[TestCase("PDT: (Пиздатые Плюшки) vs (Взрывные @everyone котюнчики)")]
	[TestCase("CFC: (mp close) vs (フフフ)")]
	[TestCase("BotB: (--... ..--- --...) vs (:monkey: but emoji)")]
	[TestCase("CC : (8mi8) vs (-- Waffle --)")]
	[TestCase("CAT18: 1k-9k L12 |(Minami-Kotori) vs (KomachiBaka)")]
	[TestCase("\"6 digit cup: (group\"\"3\"\") vs (group\"\"3\"\")\"")]
	[TestCase("AHSESPORTS: (The S.S.) vs (over salted utakus)")]
	[TestCase("BGCC2: Водоземската Мафия vs po4ina")]
	[TestCase("BGCC2: Водоземската Мафия vs. po4ina")]
	[TestCase("BGCC2: Водоземската Мафия VS. po4ina")]
	[TestCase("BGCC2: Водоземската Мафия VS po4ina")]
	public void Patterns_Regex_ShouldMatch(string name) => Assert.That(LobbyNameChecker.IsNameValid(name), Is.True);

	[Test]
	[TestCase("test (a) vs. (b)")]
	[TestCase("test (a) vs (b)")]
	[TestCase("WDTWE (:smirk_cat:) vs (RAMSING)")]
	public void Patterns_Regex_ShouldFail(string name) => Assert.That(LobbyNameChecker.IsNameValid(name), Is.False);
}