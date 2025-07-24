using AutoMapper;
using Common.Enums;
using Database.Entities;
using JetBrains.Annotations;
using OsuApiClient.Domain.Osu.Beatmaps;
using OsuApiClient.Domain.Osu.Multiplayer;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Domain.Osu.Users.Attributes;
using ApiGameScore = OsuApiClient.Domain.Osu.Multiplayer.GameScore;
using ApiUser = OsuApiClient.Domain.Osu.Users.User;
using DbBeatmap = Database.Entities.Beatmap;
using DbBeatmapset = Database.Entities.Beatmapset;
using DbGameScore = Database.Entities.GameScore;

namespace DWS.Configurations;

/// <summary>
/// AutoMapper profile for mapping between OsuApiClient domain types and database entities
/// </summary>
[UsedImplicitly]
public class DwsMapperProfile : Profile
{
    public DwsMapperProfile()
    {
        // Multiplayer mappings
        CreateMatchMappings();
        CreateGameMappings();
        CreateGameScoreMappings();

        // User mappings
        CreateUserMappings();

        // Beatmap mappings
        CreateBeatmapMappings();
    }

    /// <summary>
    /// Create mappings for multiplayer match types
    /// </summary>
    private void CreateMatchMappings()
    {
        // MultiplayerMatch → Match
        CreateMap<MultiplayerMatch, Match>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Match.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Match.Name))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Match.StartTime, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Match.EndTime.HasValue
                ? DateTime.SpecifyKind(src.Match.EndTime.Value, DateTimeKind.Utc)
                : (DateTime?)null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Tournament, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentId, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.Rosters, opt => opt.Ignore())
            .ForMember(dest => dest.PlayerRatingAdjustments, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.WinRecord, opt => opt.Ignore())
            .ForMember(dest => dest.ProcessingStatus, opt => opt.Ignore())
            .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
            .ForMember(dest => dest.WarningFlags, opt => opt.Ignore())
            .ForMember(dest => dest.VerificationStatus, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore());

        // MatchUser → Player (for creating players from match users)
        CreateMap<MatchUser, Player>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DefaultRuleset, opt => opt.Ignore())
            .ForMember(dest => dest.OsuLastFetch, opt => opt.Ignore())
            .ForMember(dest => dest.OsuTrackLastFetch, opt => opt.Ignore())
            .ForMember(dest => dest.RulesetData, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentStats, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.HighestRanks, opt => opt.Ignore());
    }

    /// <summary>
    /// Create mappings for multiplayer game types
    /// </summary>
    private void CreateGameMappings()
    {
        // MultiplayerGame → Game
        CreateMap<MultiplayerGame, Game>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.StartTime, DateTimeKind.Utc)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.EndTime ?? src.StartTime, DateTimeKind.Utc)))
            .ForMember(dest => dest.Ruleset, opt => opt.MapFrom(src => src.Ruleset))
            .ForMember(dest => dest.ScoringType, opt => opt.MapFrom(src => src.ScoringType))
            .ForMember(dest => dest.TeamType, opt => opt.MapFrom(src => src.TeamType))
            .ForMember(dest => dest.Mods, opt => opt.MapFrom(src => src.Mods))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MatchId, opt => opt.Ignore())
            .ForMember(dest => dest.Match, opt => opt.Ignore())
            .ForMember(dest => dest.BeatmapId, opt => opt.Ignore())
            .ForMember(dest => dest.Beatmap, opt => opt.Ignore())
            .ForMember(dest => dest.Scores, opt => opt.Ignore())
            .ForMember(dest => dest.Rosters, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.ProcessingStatus, opt => opt.Ignore())
            .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
            .ForMember(dest => dest.WarningFlags, opt => opt.Ignore())
            .ForMember(dest => dest.VerificationStatus, opt => opt.Ignore())
            .ForMember(dest => dest.IsFreeMod, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore());
    }

    /// <summary>
    /// Create mappings for game score types
    /// </summary>
    private void CreateGameScoreMappings()
    {
        // ApiGameScore → DbGameScore
        CreateMap<ApiGameScore, DbGameScore>()
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.SlotInfo.Team))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => CalculateScore(src.Score, src.Mods)))
            .ForMember(dest => dest.MaxCombo, opt => opt.MapFrom(src => src.MaxCombo))
            .ForMember(dest => dest.Count300, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.Count300 : 0))
            .ForMember(dest => dest.Count100, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.Count100 : 0))
            .ForMember(dest => dest.Count50, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.Count50 : 0))
            .ForMember(dest => dest.CountMiss, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.CountMiss : 0))
            .ForMember(dest => dest.CountGeki, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.CountGeki : 0))
            .ForMember(dest => dest.CountKatu, opt => opt.MapFrom(src => src.Statistics != null ? src.Statistics.CountKatu : 0))
            .ForMember(dest => dest.Pass, opt => opt.MapFrom(src => src.Passed))
            .ForMember(dest => dest.Perfect, opt => opt.MapFrom(src => src.Perfect))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade))
            .ForMember(dest => dest.Mods, opt => opt.MapFrom(src => src.Mods))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GameId, opt => opt.Ignore())
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.PlayerId, opt => opt.Ignore())
            .ForMember(dest => dest.Player, opt => opt.Ignore())
            .ForMember(dest => dest.Ruleset, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.ProcessingStatus, opt => opt.Ignore())
            .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.Accuracy, opt => opt.Ignore());
    }

    /// <summary>
    /// Create mappings for user types
    /// </summary>
    private void CreateUserMappings()
    {
        // UserExtended → Player
        CreateMap<UserExtended, Player>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.DefaultRuleset, opt => opt.MapFrom(src => src.Ruleset))
            .ForMember(dest => dest.OsuLastFetch, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OsuTrackLastFetch, opt => opt.Ignore())
            .ForMember(dest => dest.RulesetData, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentStats, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.HighestRanks, opt => opt.Ignore());

        // ApiUser → Player
        CreateMap<ApiUser, Player>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DefaultRuleset, opt => opt.Ignore())
            .ForMember(dest => dest.OsuLastFetch, opt => opt.Ignore())
            .ForMember(dest => dest.OsuTrackLastFetch, opt => opt.Ignore())
            .ForMember(dest => dest.RulesetData, opt => opt.Ignore())
            .ForMember(dest => dest.TournamentStats, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.HighestRanks, opt => opt.Ignore());

        // UserStatisticsVariant → PlayerOsuRulesetData
        CreateMap<UserStatisticsVariant, PlayerOsuRulesetData>()
            .ForMember(dest => dest.Ruleset, opt => opt.MapFrom(src => src.Ruleset))
            .ForMember(dest => dest.Pp, opt => opt.MapFrom(src => src.Pp))
            .ForMember(dest => dest.GlobalRank, opt => opt.MapFrom(src => src.GlobalRank!.Value))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PlayerId, opt => opt.Ignore())
            .ForMember(dest => dest.Player, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore());
    }

    /// <summary>
    /// Create mappings for beatmap types
    /// </summary>
    private void CreateBeatmapMappings()
    {
        // BeatmapExtended → DbBeatmap
        CreateMap<BeatmapExtended, DbBeatmap>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.HasData, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Ruleset, opt => opt.MapFrom(src => src.Ruleset))
            .ForMember(dest => dest.RankedStatus, opt => opt.MapFrom(src => src.RankedStatus))
            .ForMember(dest => dest.DiffName, opt => opt.MapFrom(src => src.DifficultyName))
            .ForMember(dest => dest.TotalLength, opt => opt.MapFrom(src => src.TotalLength))
            .ForMember(dest => dest.DrainLength, opt => opt.MapFrom(src => src.HitLength))
            .ForMember(dest => dest.Bpm, opt => opt.MapFrom(src => src.Bpm))
            .ForMember(dest => dest.CountCircle, opt => opt.MapFrom(src => src.CountCircles))
            .ForMember(dest => dest.CountSlider, opt => opt.MapFrom(src => src.CountSliders))
            .ForMember(dest => dest.CountSpinner, opt => opt.MapFrom(src => src.CountSpinners))
            .ForMember(dest => dest.Cs, opt => opt.MapFrom(src => src.CircleSize))
            .ForMember(dest => dest.Hp, opt => opt.MapFrom(src => src.HpDrain))
            .ForMember(dest => dest.Od, opt => opt.MapFrom(src => src.OverallDifficulty))
            .ForMember(dest => dest.Ar, opt => opt.MapFrom(src => src.ApproachRate))
            .ForMember(dest => dest.Sr, opt => opt.MapFrom(src => src.StarRating))
            .ForMember(dest => dest.MaxCombo, opt => opt.MapFrom(src => src.MaxCombo))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BeatmapsetId, opt => opt.Ignore())
            .ForMember(dest => dest.Beatmapset, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.Attributes, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore());

        // BeatmapsetExtended → DbBeatmapset
        CreateMap<BeatmapsetExtended, DbBeatmapset>()
            .ForMember(dest => dest.OsuId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => src.Artist))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.RankedStatus, opt => opt.MapFrom(src => src.RankedStatus))
            .ForMember(dest => dest.RankedDate, opt => opt.MapFrom(src => src.RankedDate))
            .ForMember(dest => dest.SubmittedDate, opt => opt.MapFrom(src => src.SubmittedDate))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore())
            .ForMember(dest => dest.Beatmaps, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore());
    }

    /// <summary>
    /// Calculate the final score, applying Easy mod multiplier if needed
    /// </summary>
    /// <param name="rawScore">The raw score from the API</param>
    /// <param name="mods">The mods used in the score</param>
    /// <returns>The calculated score</returns>
    private static int CalculateScore(int rawScore, Mods mods)
    {
        return mods.HasFlag(Mods.Easy) ? (int)(rawScore * 1.75) : rawScore;
    }
}
