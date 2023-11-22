namespace API.Utilities;

public static class RatingUtils
{
	// Players at or below the threshold are considered to be in the previous tier
	
	// Bronze: 100-499
	public const int RatingSilver = 500;
	public const int RatingGold = 800;
	public const int RatingPlatinum = 1100;
	public const int RatingRuby = 1400;
	public const int RatingDiamond = 1700;
	public const int RatingMaster = 2000;
	public const int RatingGrandmaster = 2300;
	public const int RatingEliteGrandmaster = 2600;
	
	public static string GetTier(double rating) => rating switch
	{
		< RatingSilver => "Bronze",
		< RatingGold => "Silver",
		< RatingPlatinum => "Gold",
		< RatingRuby => "Platinum",
		< RatingDiamond => "Ruby",
		< RatingMaster => "Diamond",
		< RatingGrandmaster => "Master",
		< RatingEliteGrandmaster => "Grandmaster",
		_ => "Elite Grandmaster"
	};
	
	public static string GetNextTier(double rating) => rating switch
	{
		< RatingSilver => GetTier(RatingSilver),
		< RatingGold => GetTier(RatingGold),
		< RatingPlatinum => GetTier(RatingPlatinum),
		< RatingRuby => GetTier(RatingRuby),
		< RatingDiamond => GetTier(RatingDiamond),
		< RatingMaster => GetTier(RatingMaster),
		< RatingGrandmaster => GetTier(RatingGrandmaster),
		< RatingEliteGrandmaster => GetTier(RatingEliteGrandmaster),
		_ => GetTier(RatingEliteGrandmaster)
	};

	public static double GetRatingForNextTier(double rating) => rating switch
	{
		< RatingSilver => RatingSilver - rating,
		< RatingGold => RatingGold - rating,
		< RatingPlatinum => RatingPlatinum - rating,
		< RatingRuby => RatingRuby - rating,
		< RatingDiamond => RatingDiamond - rating,
		< RatingMaster => RatingMaster - rating,
		< RatingGrandmaster => RatingGrandmaster - rating,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - rating,
		_ => 0
	};
	
	// Useful for the front-end rating progress bar
	/// <summary>
	/// Returns the difference between the ratings required for the next tier and the current tier.
	/// This is used by the front-end's progress bar to determine how much progress has been made towards the next tier.
	/// </summary>
	/// <param name="rating"></param>
	/// <returns></returns>
	public static double GetRatingDelta(double rating) => rating switch
	{
		< RatingSilver => RatingSilver/* - RatingBronze*/,
		< RatingGold => RatingGold - RatingSilver,
		< RatingPlatinum => RatingPlatinum - RatingGold,
		< RatingRuby => RatingRuby - RatingPlatinum,
		< RatingDiamond => RatingDiamond - RatingRuby,
		< RatingMaster => RatingMaster - RatingDiamond,
		< RatingGrandmaster => RatingGrandmaster - RatingMaster,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - RatingGrandmaster,
		_ => 0
	};
}