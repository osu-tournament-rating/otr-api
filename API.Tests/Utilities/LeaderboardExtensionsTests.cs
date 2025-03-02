using API.DTOs;
using API.Utilities;
using API.Utilities.Extensions;
using Database.Models;

namespace APITests.Utilities;

public class LeaderboardExtensionsTests
{
    [Fact]
    public void IsEngaged_NullFilter_ReturnsFalse()
    {
        LeaderboardTierFilter? filter = null;
        Assert.False(filter.IsEngaged());
    }

    [Fact]
    public void IsEngaged_EmptyFilter_ReturnsFalse()
    {
        var filter = new LeaderboardTierFilter();
        Assert.False(filter.IsEngaged());
    }

    [Fact]
    public void IsEngaged_FilterWithAnyTrue_ReturnsTrue()
    {
        var filter = new LeaderboardTierFilter
        {
            FilterBronze = true
        };

        var filter2 = new LeaderboardTierFilter
        {
            FilterSilver = true
        };

        var filter3 = new LeaderboardTierFilter
        {
            FilterGold = true
        };

        var filter4 = new LeaderboardTierFilter
        {
            FilterPlatinum = true
        };

        var filter5 = new LeaderboardTierFilter
        {
            FilterEmerald = true
        };

        var filter6 = new LeaderboardTierFilter
        {
            FilterDiamond = true
        };

        var filter7 = new LeaderboardTierFilter
        {
            FilterMaster = true
        };

        var filter8 = new LeaderboardTierFilter
        {
            FilterGrandmaster = true
        };

        var filter9 = new LeaderboardTierFilter
        {
            FilterEliteGrandmaster = true
        };

        Assert.Multiple(() =>
        {
            Assert.True(filter.IsEngaged());
            Assert.True(filter2.IsEngaged());
            Assert.True(filter3.IsEngaged());
            Assert.True(filter4.IsEngaged());
            Assert.True(filter5.IsEngaged());
            Assert.True(filter6.IsEngaged());
            Assert.True(filter7.IsEngaged());
            Assert.True(filter8.IsEngaged());
            Assert.True(filter9.IsEngaged());
        });
    }

    [Fact]
    public void IsEliteGrandmasterTier_TierIsEliteGrandmaster_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Elite Grandmaster"
        };

        Assert.True(RatingUtils.IsEliteGrandmaster(lbInfo.Tier));
    }

    [Fact]
    public void IsGrandmasterTier_TierIsGrandmaster_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Grandmaster I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Grandmaster II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Grandmaster III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsGrandmaster(lbInfo.Tier));
            Assert.True(RatingUtils.IsGrandmaster(lbInfo2.Tier));
            Assert.True(RatingUtils.IsGrandmaster(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsMasterTier_TierIsMaster_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Master I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Master II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Master III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsMaster(lbInfo.Tier));
            Assert.True(RatingUtils.IsMaster(lbInfo2.Tier));
            Assert.True(RatingUtils.IsMaster(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsDiamondTier_TierIsDiamond_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Diamond I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Diamond II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Diamond III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsDiamond(lbInfo.Tier));
            Assert.True(RatingUtils.IsDiamond(lbInfo2.Tier));
            Assert.True(RatingUtils.IsDiamond(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsEmeraldTier_TierIsEmerald_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Emerald I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Emerald II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Emerald III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsEmerald(lbInfo.Tier));
            Assert.True(RatingUtils.IsEmerald(lbInfo2.Tier));
            Assert.True(RatingUtils.IsEmerald(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsPlatinumTier_TierIsPlatinum_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Platinum I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Platinum II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Platinum III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsPlatinum(lbInfo.Tier));
            Assert.True(RatingUtils.IsPlatinum(lbInfo2.Tier));
            Assert.True(RatingUtils.IsPlatinum(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsGoldTier_TierIsGold_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Gold I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Gold II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Gold III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsGold(lbInfo.Tier));
            Assert.True(RatingUtils.IsGold(lbInfo2.Tier));
            Assert.True(RatingUtils.IsGold(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsSilverTier_TierIsSilver_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Silver I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Silver II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Silver III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsSilver(lbInfo.Tier));
            Assert.True(RatingUtils.IsSilver(lbInfo2.Tier));
            Assert.True(RatingUtils.IsSilver(lbInfo3.Tier));
        });
    }

    [Fact]
    public void IsBronzeTier_TierIsBronze_ReturnsTrue()
    {
        var lbInfo = new LeaderboardPlayerInfoDTO
        {
            Tier = "Bronze I"
        };

        var lbInfo2 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Bronze II"
        };

        var lbInfo3 = new LeaderboardPlayerInfoDTO
        {
            Tier = "Bronze III"
        };

        Assert.Multiple(() =>
        {
            Assert.True(RatingUtils.IsBronze(lbInfo.Tier));
            Assert.True(RatingUtils.IsBronze(lbInfo2.Tier));
            Assert.True(RatingUtils.IsBronze(lbInfo3.Tier));
        });
    }
}
