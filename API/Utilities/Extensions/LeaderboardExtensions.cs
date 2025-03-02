using Database.Models;

namespace API.Utilities.Extensions;

public static class LeaderboardExtensions
{
    public static bool IsEngaged(this LeaderboardTierFilter? filter)
    {
        if (filter is null)
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
