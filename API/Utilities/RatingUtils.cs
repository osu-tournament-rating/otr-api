namespace API.Utilities;

public static class RatingUtils
{
	public const int RatingBronze = 499;
	public const int RatingSilver = 899;
	public const int RatingGold = 1299;
	public const int RatingPlatinum = 1599;
	public const int RatingDiamond = 1899;
	public const int RatingMaster = 2199;
	public const int RatingGrandmaster = 2499;
	public const int RatingEliteGrandmaster = 2799;
	
	public static string GetTier(int rating) => rating switch
	{
		< RatingSilver => "Bronze",
		< RatingGold => "Silver",
		< RatingPlatinum => "Gold",
		< RatingDiamond => "Platinum",
		< RatingMaster => "Diamond",
		< RatingGrandmaster => "Master",
		< RatingEliteGrandmaster => "Grandmaster",
		_ => "Elite Grandmaster"
	};
	
	public static string GetNextTier(int rating) => rating switch
	{
		< RatingSilver => GetTier(RatingSilver),
		< RatingGold => GetTier(RatingGold),
		< RatingPlatinum => GetTier(RatingPlatinum),
		< RatingDiamond => GetTier(RatingDiamond),
		< RatingMaster => GetTier(RatingMaster),
		< RatingGrandmaster => GetTier(RatingGrandmaster),
		< RatingEliteGrandmaster => GetTier(RatingEliteGrandmaster),
		_ => GetTier(RatingEliteGrandmaster)
	};

	public static int GetRatingForNextTier(int rating) => rating switch
	{
		< RatingSilver => RatingSilver - rating,
		< RatingGold => RatingGold - rating,
		< RatingPlatinum => RatingPlatinum - rating,
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
	public static int GetRatingDelta(int rating) => rating switch
	{
		< RatingSilver => RatingSilver - RatingBronze,
		< RatingGold => RatingGold - RatingSilver,
		< RatingPlatinum => RatingPlatinum - RatingGold,
		< RatingDiamond => RatingDiamond - RatingPlatinum,
		< RatingMaster => RatingMaster - RatingDiamond,
		< RatingGrandmaster => RatingGrandmaster - RatingMaster,
		< RatingEliteGrandmaster => RatingEliteGrandmaster - RatingGrandmaster,
		_ => 0
	};
}