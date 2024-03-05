using API.DTOs;

namespace API.Utilities;

public static class LeaderboardExtensions
{
    public static bool IsInvalid(this LeaderboardTierFilterDTO? filter)
    {
        if (filter == null)
        {
            return false;
        }

        if (
            filter.FilterBronze == false
            && filter.FilterSilver == false
            && filter.FilterGold == false
            && filter.FilterPlatinum == false
            && filter.FilterEmerald == false
            && filter.FilterDiamond == false
            && filter.FilterMaster == false
            && filter.FilterGrandmaster == false
            && filter.FilterEliteGrandmaster == false
        )
        {
            return true;
        }

        return false;
    }
}
