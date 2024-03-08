using API.DTOs;
using API.Entities;
using API.Utilities;

namespace APITests.SeedData;

public static class SeededBaseStats
{
    public static BaseStats Get() =>
        new()
        {
            Id = 1,
            PlayerId = 1,
            Mode = 0,
            Rating = 1245.324,
            Volatility = 100.5231,
            Percentile = 0.3431,
            GlobalRank = 20,
            CountryRank = 2,
            MatchCostAverage = 1.23424,
            Created = new DateTime(2023, 11, 11),
            Updated = new DateTime(2023, 11, 12)
        };

    public static List<BaseStats> GetSimpleLeaderboard(int size = 25)
    {
        var lb = new List<BaseStats>();
        for (int i = 0; i < size; i++)
        {
            lb.Add(Get());
        }

        return lb;
    }

    public static List<BaseStats> GetLeaderboardFiltered(LeaderboardFilterDTO filter, int size = 25)
    {
        var lb = new List<BaseStats>();
        var tiers = filter.TierFilters;

        if (tiers == null)
        {
            return GetSimpleLeaderboard();
        }

        if (tiers.IsInvalid())
        {
            throw new ArgumentException("The tier filter is invalid");
        }

        // Add all of the tiers that are true
        if (tiers.FilterEliteGrandmaster == true)
        {
            lb.Add(Get().SetEliteGrandmaster());
        }

        if (tiers.FilterGrandmaster == true)
        {
            lb.Add(Get().SetGrandmaster());
        }

        if (tiers.FilterMaster == true)
        {
            lb.Add(Get().SetMaster());
        }

        if (tiers.FilterDiamond == true)
        {
            lb.Add(Get().SetDiamond());
        }

        if (tiers.FilterEmerald == true)
        {
            lb.Add(Get().SetEmerald());
        }

        if (tiers.FilterPlatinum == true)
        {
            lb.Add(Get().SetPlatinum());
        }

        if (tiers.FilterGold == true)
        {
            lb.Add(Get().SetGold());
        }

        if (tiers.FilterSilver == true)
        {
            lb.Add(Get().SetSilver());
        }

        if (tiers.FilterBronze == true)
        {
            lb.Add(Get().SetBronze());
        }

        // Get the first item's fill rating because it's guaranteed to be allowed
        double fillRating = lb.First().Rating;
        int fillAmount = size - lb.Count;

        for (int i = 0; i < fillAmount; i++)
        {
            var toAdd = Get();
            toAdd.Rating = fillRating;

            lb.Add(toAdd);
        }

        // Remove the tiers that are false
        if (tiers.FilterEliteGrandmaster == false)
        {
            lb.RemoveAll(x => x.Rating >= RatingUtils.RatingEliteGrandmaster);
        }

        if (tiers.FilterGrandmaster == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingGrandmasterIII && x.Rating < RatingUtils.RatingEliteGrandmaster
            );
        }

        if (tiers.FilterMaster == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingMasterIII && x.Rating < RatingUtils.RatingGrandmasterIII
            );
        }

        if (tiers.FilterDiamond == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingDiamondIII && x.Rating < RatingUtils.RatingMasterIII
            );
        }

        if (tiers.FilterEmerald == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingEmeraldIII && x.Rating < RatingUtils.RatingDiamondIII
            );
        }

        if (tiers.FilterPlatinum == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingPlatinumIII && x.Rating < RatingUtils.RatingEmeraldIII
            );
        }

        if (tiers.FilterGold == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingGoldIII && x.Rating < RatingUtils.RatingPlatinumIII
            );
        }

        if (tiers.FilterSilver == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingSilverIII && x.Rating < RatingUtils.RatingGoldIII
            );
        }

        if (tiers.FilterBronze == false)
        {
            lb.RemoveAll(x =>
                x.Rating >= RatingUtils.RatingBronzeIII && x.Rating < RatingUtils.RatingSilverIII
            );
        }

        return lb;
    }

    private static BaseStats SetEliteGrandmaster(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingEliteGrandmaster;
        return b;
    }

    private static BaseStats SetGrandmaster(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingGrandmasterIII;
        return b;
    }

    private static BaseStats SetMaster(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingMasterIII;
        return b;
    }

    private static BaseStats SetDiamond(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingDiamondIII;
        return b;
    }

    private static BaseStats SetEmerald(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingEmeraldIII;
        return b;
    }

    private static BaseStats SetPlatinum(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingPlatinumIII;
        return b;
    }

    private static BaseStats SetGold(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingGoldIII;
        return b;
    }

    private static BaseStats SetSilver(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingSilverIII;
        return b;
    }

    private static BaseStats SetBronze(this BaseStats b)
    {
        b.Rating = RatingUtils.RatingBronzeIII;
        return b;
    }
}
