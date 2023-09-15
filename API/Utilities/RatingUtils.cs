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
	
	public static string GetRankingClassName(int rating) => rating switch
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
	
	public static string GetNextRankingClassName(int rating) => rating switch
	{
		< RatingSilver => GetRankingClassName(RatingSilver),
		< RatingGold => GetRankingClassName(RatingGold),
		< RatingPlatinum => GetRankingClassName(RatingPlatinum),
		< RatingDiamond => GetRankingClassName(RatingDiamond),
		< RatingMaster => GetRankingClassName(RatingMaster),
		< RatingGrandmaster => GetRankingClassName(RatingGrandmaster),
		< RatingEliteGrandmaster => GetRankingClassName(RatingEliteGrandmaster),
		_ => GetRankingClassName(RatingEliteGrandmaster)
	};

	public static int GetRatingNeededForNextRank(int rating) => rating switch
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