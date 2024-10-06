using API.DTOs;
using API.Utilities;
using API.Utilities.Extensions;
using Database.Entities.Processor;

namespace APITests.SeedData;

public static class SeededBaseStats
{
    public static PlayerRating Get() =>
        new()
        {
            Id = 1,
            PlayerId = 1,
            Ruleset = 0,
            Rating = 1245.324,
            Volatility = 100.5231,
            Percentile = 0.3431,
            GlobalRank = 20,
            CountryRank = 2,
            Created = new DateTime(2023, 11, 11)
        };

    public static List<PlayerRating> GetSimpleLeaderboard(int size = 25)
    {
        var lb = new List<PlayerRating>();
        for (var i = 0; i < size; i++)
        {
            lb.Add(Get());
        }

        return lb;
    }

    public static List<PlayerRating> GetLeaderboardFiltered(LeaderboardFilterDTO filter, int size = 25)
    {
        var lb = new List<PlayerRating>();
        LeaderboardTierFilterDTO? tiers = filter.TierFilters;

        if (tiers == null || !tiers.IsEngaged())
        {
            return GetSimpleLeaderboard();
        }

        // Add all of the tiers that are true
        if (tiers.FilterEliteGrandmaster)
        {
            lb.Add(Get().SetEliteGrandmaster());
        }

        if (tiers.FilterGrandmaster)
        {
            lb.Add(Get().SetGrandmaster());
        }

        if (tiers.FilterMaster)
        {
            lb.Add(Get().SetMaster());
        }

        if (tiers.FilterDiamond)
        {
            lb.Add(Get().SetDiamond());
        }

        if (tiers.FilterEmerald)
        {
            lb.Add(Get().SetEmerald());
        }

        if (tiers.FilterPlatinum)
        {
            lb.Add(Get().SetPlatinum());
        }

        if (tiers.FilterGold)
        {
            lb.Add(Get().SetGold());
        }

        if (tiers.FilterSilver)
        {
            lb.Add(Get().SetSilver());
        }

        if (tiers.FilterBronze)
        {
            lb.Add(Get().SetBronze());
        }

        // Get the first item's fill rating because it's guaranteed to be allowed
        var fillRating = lb.First().Rating;
        var fillAmount = size - lb.Count;

        for (var i = 0; i < fillAmount; i++)
        {
            PlayerRating toAdd = Get();
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

    private static PlayerRating SetEliteGrandmaster(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingEliteGrandmaster;
        return b;
    }

    private static PlayerRating SetGrandmaster(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingGrandmasterIII;
        return b;
    }

    private static PlayerRating SetMaster(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingMasterIII;
        return b;
    }

    private static PlayerRating SetDiamond(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingDiamondIII;
        return b;
    }

    private static PlayerRating SetEmerald(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingEmeraldIII;
        return b;
    }

    private static PlayerRating SetPlatinum(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingPlatinumIII;
        return b;
    }

    private static PlayerRating SetGold(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingGoldIII;
        return b;
    }

    private static PlayerRating SetSilver(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingSilverIII;
        return b;
    }

    private static PlayerRating SetBronze(this PlayerRating b)
    {
        b.Rating = RatingUtils.RatingBronzeIII;
        return b;
    }
}
