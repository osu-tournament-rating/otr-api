using API.Osu;

namespace APITests.Tests.Osu;

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
    public void Patterns_Regex_ShouldMatch(string name, string abbreviation) =>
        Assert.True(LobbyNameChecker.IsNameValid(name, abbreviation));

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
    public void Patterns_Regex_ShouldNotMatch(string name, string abbreviation) =>
        Assert.False(LobbyNameChecker.IsNameValid(name, abbreviation));

    [Fact]
    public void Patterns_ReturnsFalse_WhenNameIsNull() =>
        Assert.False(LobbyNameChecker.IsNameValid(null!, "test"));

    [Fact]
    public void Patterns_ReturnsFalse_WhenNameIsEmpty() =>
        Assert.False(LobbyNameChecker.IsNameValid(string.Empty, "test"));

    [Fact]
    public void Patterns_ReturnsFalse_WhenAbbreviationIsNull() =>
        Assert.False(LobbyNameChecker.IsNameValid("test", null!));

    [Fact]
    public void Patterns_ReturnsFalse_WhenAbbreviationIsEmpty() =>
        Assert.False(LobbyNameChecker.IsNameValid("test", string.Empty));

    [Fact]
    public void NameCheck_ReturnsFalse_WhenNameDoesNotStartWithAbbreviation() =>
        Assert.False(LobbyNameChecker.IsNameValid("test", "test2"));

    [Fact]
    public void NameCheck_ReturnsFalse_WhenMatchAbbreviationIsNull() =>
        Assert.False(LobbyNameChecker.IsNameValid("test", null!));

    [Fact]
    public void NameCheck_ReturnsFalse_WhenMatchAbbreviationIsEmpty() =>
        Assert.False(LobbyNameChecker.IsNameValid("test", string.Empty));

    [Fact]
    public void NameCheck_ReturnsFalse_WhenMatchNameIsNull() =>
        Assert.False(LobbyNameChecker.IsNameValid(null!, "test"));

    [Fact]
    public void NameCheck_ReturnsFalse_WhenMatchNameIsEmpty() =>
        Assert.False(LobbyNameChecker.IsNameValid(string.Empty, "test"));

    [Fact]
    public void NameCheck_ReturnsTrue_IgnoreCase() =>
        Assert.True(LobbyNameChecker.IsNameValid("test: (A) vs (B)", "TEST"));
}
