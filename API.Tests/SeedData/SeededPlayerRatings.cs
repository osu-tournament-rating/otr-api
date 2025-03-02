using API.DTOs;
using API.Utilities;
using API.Utilities.Extensions;
using Common.Rating;
using Database.Entities.Processor;
using Database.Models;

namespace APITests.SeedData;

public static class SeededPlayerRatings
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

    public static List<PlayerRating> GetLeaderboardFiltered(LeaderboardFilter filter, int size = 25)
    {
        var lb = new List<PlayerRating>();
        LeaderboardTierFilter? tiers = filter.TierFilters;

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
            lb.RemoveAll(x => x.Rating >= RatingConstants.RatingEliteGrandmaster);
        }

        if (tiers.FilterGrandmaster == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingGrandmasterIII and < RatingConstants.RatingEliteGrandmaster
            );
        }

        if (tiers.FilterMaster == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingMasterIII and < RatingConstants.RatingGrandmasterIII
            );
        }

        if (tiers.FilterDiamond == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingDiamondIII and < RatingConstants.RatingMasterIII
            );
        }

        if (tiers.FilterEmerald == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingEmeraldIII and < RatingConstants.RatingDiamondIII
            );
        }

        if (tiers.FilterPlatinum == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingPlatinumIII and < RatingConstants.RatingEmeraldIII
            );
        }

        if (tiers.FilterGold == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingGoldIII and < RatingConstants.RatingPlatinumIII
            );
        }

        if (tiers.FilterSilver == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingSilverIII and < RatingConstants.RatingGoldIII
            );
        }

        if (tiers.FilterBronze == false)
        {
            lb.RemoveAll(x =>
                x.Rating is >= RatingConstants.RatingBronzeIII and < RatingConstants.RatingSilverIII
            );
        }

        return lb;
    }

    private static PlayerRating SetEliteGrandmaster(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingEliteGrandmaster;
        return b;
    }

    private static PlayerRating SetGrandmaster(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingGrandmasterIII;
        return b;
    }

    private static PlayerRating SetMaster(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingMasterIII;
        return b;
    }

    private static PlayerRating SetDiamond(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingDiamondIII;
        return b;
    }

    private static PlayerRating SetEmerald(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingEmeraldIII;
        return b;
    }

    private static PlayerRating SetPlatinum(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingPlatinumIII;
        return b;
    }

    private static PlayerRating SetGold(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingGoldIII;
        return b;
    }

    private static PlayerRating SetSilver(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingSilverIII;
        return b;
    }

    private static PlayerRating SetBronze(this PlayerRating b)
    {
        b.Rating = RatingConstants.RatingBronzeIII;
        return b;
    }
}
