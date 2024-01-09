using API.Utilities;

namespace API.DTOs;

/// <summary>
/// General stats that are current and not time specific, i.e. current rating, volatility, tier 
/// </summary>
public class BaseStatsDTO
{
	public BaseStatsDTO(int playerId, double rating, double volatility, int mode, double percentile,
		int matchesPlayed, double winrate, int highestGlobalRank, int globalRank, int countryRank, double averageMatchCost)
	{
		PlayerId = playerId;
		Rating = rating;
		Volatility = volatility;
		Mode = mode;
		Percentile = percentile;
		MatchesPlayed = matchesPlayed;
		Winrate = winrate;
		HighestGlobalRank = highestGlobalRank;
		GlobalRank = globalRank;
		CountryRank = countryRank;
		AverageMatchCost = averageMatchCost;
	}
	
	public int PlayerId { get; set; }
	public double Rating { get; set; }
	public double Volatility { get; set; }
	public int Mode { get; set; }
	public double Percentile { get; set; }
	public int MatchesPlayed { get; set; }
	public double Winrate { get; set; }
	public int HighestGlobalRank { get; set; }
	public int GlobalRank { get; set; }
	public int CountryRank { get; set; }
	public double AverageMatchCost { get; set; }

	public string Tier => RatingUtils.GetTier((int) Rating);
	public string NextTier => RatingUtils.GetNextTier((int) Rating);
	
	public double RatingForNextTier => RatingUtils.GetRatingForNextTier((int) Rating);
	public double RatingDelta => RatingUtils.GetRatingDelta((int) Rating);
}