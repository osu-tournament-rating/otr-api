using API.DTOs;

namespace API.Utilities;

public static class LeaderboardExtensions
{
    public static bool IsEngaged(this LeaderboardTierFilterDTO? filter)
    {
        if (filter == null)
        {
            return false;
        }

        return filter.FilterBronze
               || filter.FilterSilver
               || filter.FilterGold
               || filter.FilterPlatinum
               || filter.FilterEmerald
               || filter.FilterDiamond
               || filter.FilterMaster
               || filter.FilterGrandmaster
               || filter.FilterEliteGrandmaster;
    }
}
