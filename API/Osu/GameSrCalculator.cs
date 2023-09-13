using API.Services.Interfaces;

namespace API.Osu;

public class GameSrCalculator : IGameSrCalculator
{
	private readonly IBeatmapService _beatmapService;

	public GameSrCalculator(IBeatmapService beatmapService) { _beatmapService = beatmapService; }
	
	public async Task<double> Calculate(double baseSr, int beatmapId, OsuEnums.Mods baseMods, IEnumerable<OsuEnums.Mods?> playerAppliedMods)
	{
		bool freeMod = baseMods.HasFlag(OsuEnums.Mods.FreeModAllowed);

		if (!freeMod)
		{
			if (baseMods.HasFlag(OsuEnums.Mods.DoubleTime))
			{
				return await _beatmapService.GetDoubleTimeSrAsync(beatmapId);
			}

			if (baseMods.HasFlag(OsuEnums.Mods.HardRock))
			{
				return await _beatmapService.GetHardRockSrAsync(beatmapId);
			}

			if (baseMods.HasFlag(OsuEnums.Mods.Easy))
			{
				return await _beatmapService.GetEasySrAsync(beatmapId);
			}

			if (baseMods.HasFlag(OsuEnums.Mods.HalfTime))
			{
				return await _beatmapService.GetHalfTimeSrAsync(beatmapId);
			}

			return baseSr;
		}

		bool containsHardRock = playerAppliedMods.Any(x => x.HasValue && x.Value.HasFlag(OsuEnums.Mods.HardRock));
		// HR is forced for at least one player, thus we count the HR SR as
		// it is assumed the other mod combinations are loosely equivalent to HR in difficulty
		if (containsHardRock)
		{
			return await _beatmapService.GetHardRockSrAsync(beatmapId);
		}

		// If all else fails, return the base SR (this can happen quite frequently i.e. NM maps)
		return baseSr;
	}
}