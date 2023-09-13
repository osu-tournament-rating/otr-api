namespace API.Osu;

public interface IGameSrCalculator
{
	Task<double> Calculate(double baseSr, int beatmapId, OsuEnums.Mods baseMods, IEnumerable<OsuEnums.Mods?> playerAppliedMods);
}