using API.DTOs;
using API.Utilities;
using Common.Enums;
using Common.Enums.Verification;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APITests.Integration;

public class PlatformStatsIntegrationTests
{
    [Fact]
    public void Complete_PlatformStats_Serialization_Should_Use_Numeric_Enum_Keys()
    {
        // Arrange - Use the same JsonSerializerSettings as the API
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new NewtonsoftEnumDictionaryKeyConverter() }
        };

        // Create comprehensive platform stats data with all enum dictionary keys
        var platformStats = new PlatformStatsDTO
        {
            RatingStats = new RatingPlatformStatsDTO
            {
                RatingsByRuleset = new Dictionary<Ruleset, Dictionary<int, int>>
                {
                    [Ruleset.Osu] = new() { [100] = 50, [200] = 30 },
                    [Ruleset.Taiko] = new() { [150] = 20 },
                    [Ruleset.Catch] = new() { [175] = 15 },
                    [Ruleset.Mania4k] = new() { [125] = 25 },
                    [Ruleset.Mania7k] = new() { [180] = 10 }
                }
            },
            TournamentStats = new TournamentPlatformStatsDTO
            {
                TotalCount = 500,
                CountByVerificationStatus = new Dictionary<VerificationStatus, int>
                {
                    [VerificationStatus.None] = 10,
                    [VerificationStatus.PreRejected] = 20,
                    [VerificationStatus.PreVerified] = 100,
                    [VerificationStatus.Rejected] = 50,
                    [VerificationStatus.Verified] = 320
                },
                VerifiedByRuleset = new Dictionary<Ruleset, int>
                {
                    [Ruleset.Osu] = 200,
                    [Ruleset.Taiko] = 50,
                    [Ruleset.Catch] = 40,
                    [Ruleset.Mania4k] = 20,
                    [Ruleset.Mania7k] = 10
                }
            },
            UserStats = new UserPlatformStatsDTO()
        };

        // Act
        string json = JsonConvert.SerializeObject(platformStats, settings);

        // Assert - All enum keys should be numeric

        // Ruleset enum values (0, 1, 2, 4, 5)
        Assert.Contains("\"0\":", json); // Ruleset.Osu = 0
        Assert.Contains("\"1\":", json); // Ruleset.Taiko = 1  
        Assert.Contains("\"2\":", json); // Ruleset.Catch = 2
        Assert.Contains("\"4\":", json); // Ruleset.Mania4k = 4
        Assert.Contains("\"5\":", json); // Ruleset.Mania7k = 5

        // VerificationStatus enum values (0, 1, 2, 3, 4)
        Assert.Contains("\"3\":", json); // VerificationStatus.Rejected = 3 (unique value)

        // Should NOT contain string enum representations
        Assert.DoesNotContain("\"osu\":", json);
        Assert.DoesNotContain("\"taiko\":", json);
        Assert.DoesNotContain("\"fruits\":", json);
        Assert.DoesNotContain("\"mania\":", json);
        Assert.DoesNotContain("\"none\":", json);
        Assert.DoesNotContain("\"preRejected\":", json);
        Assert.DoesNotContain("\"preVerified\":", json);
        Assert.DoesNotContain("\"rejected\":", json);
        Assert.DoesNotContain("\"verified\":", json);
    }

    [Fact]
    public void Complete_Deserialization_Should_Handle_Numeric_Enum_Keys()
    {
        // Arrange
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new NewtonsoftEnumDictionaryKeyConverter() }
        };

        const string json = """
        {
            "tournamentStats": {
                "totalCount": 100,
                "countByVerificationStatus": {
                    "0": 5,
                    "1": 10,
                    "2": 30,
                    "3": 15,
                    "4": 40
                },
                "verifiedByYear": {},
                "verifiedByRuleset": {
                    "0": 25,
                    "1": 10,
                    "2": 5
                },
                "verifiedByLobbySize": {}
            },
            "ratingStats": {
                "ratingsByRuleset": {
                    "0": { "100": 20, "200": 30 },
                    "2": { "150": 15 }
                }
            },
            "userStats": {
                "sumByDate": {}
            }
        }
        """;

        // Act
        PlatformStatsDTO? result = JsonConvert.DeserializeObject<PlatformStatsDTO>(json, settings);

        // Assert
        Assert.NotNull(result);

        // Check RatingStats
        Assert.NotNull(result.RatingStats);
        Assert.Equal(2, result.RatingStats.RatingsByRuleset.Count);
        Assert.True(result.RatingStats.RatingsByRuleset.ContainsKey(Ruleset.Osu));
        Assert.True(result.RatingStats.RatingsByRuleset.ContainsKey(Ruleset.Catch));
        Assert.Equal(20, result.RatingStats.RatingsByRuleset[Ruleset.Osu][100]);
        Assert.Equal(15, result.RatingStats.RatingsByRuleset[Ruleset.Catch][150]);

        // Check TournamentStats
        Assert.NotNull(result.TournamentStats);
        Assert.Equal(5, result.TournamentStats.CountByVerificationStatus.Count);
        Assert.Equal(5, result.TournamentStats.CountByVerificationStatus[VerificationStatus.None]);
        Assert.Equal(40, result.TournamentStats.CountByVerificationStatus[VerificationStatus.Verified]);

        Assert.Equal(3, result.TournamentStats.VerifiedByRuleset.Count);
        Assert.Equal(25, result.TournamentStats.VerifiedByRuleset[Ruleset.Osu]);
        Assert.Equal(5, result.TournamentStats.VerifiedByRuleset[Ruleset.Catch]);
    }
}
