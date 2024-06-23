using AutoMapper;
using OsuApiClient.Domain.Beatmaps;
using OsuApiClient.Domain.Multiplayer;
using OsuApiClient.Domain.Site;
using OsuApiClient.Domain.Users;
using OsuApiClient.Domain.Users.Attributes;
using OsuApiClient.Net.Deserialization;
using OsuApiClient.Net.Deserialization.ValueConverters;
using OsuApiClient.Net.JsonModels.Beatmaps;
using OsuApiClient.Net.JsonModels.Multiplayer;
using OsuApiClient.Net.JsonModels.Site;
using OsuApiClient.Net.JsonModels.Users;
using OsuApiClient.Net.JsonModels.Users.Attributes;

namespace OsuApiClient.Configurations.Implementations;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Fluent mappings should be used for derived models as AutoMapper will invalidate
        // mappings when derived classes inherit the [AutoMap] attribute from the base class
        CreateMap<UserJsonModel, User>();
        CreateMap<UserExtendedJsonModel, UserExtended>()
            .ForMember(dest => dest.Ruleset, opt => opt.ConvertUsing(new RulesetConverter(), src => src.PlayMode));

        CreateMap<GroupJsonModel, Group>()
            .ForMember(dest => dest.HasRulesets, opt => opt.MapFrom(src => src.HasPlayModes));
        CreateMap<UserGroupJsonModel, UserGroup>()
            .IncludeBase<GroupJsonModel, Group>()
            .ForMember(dest => dest.Rulesets, opt => opt.MapFrom(src => src.PlayModes != null ? src.PlayModes.Select(r => RulesetConverter.Convert(r, null)) : null));

        CreateMap<UserStatisticsVariantJsonModel, UserStatisticsVariant>()
            .ForMember(dest => dest.Ruleset, opt => opt.MapFrom(src => RulesetConverter.Convert(src.Mode, src.Variant)));

        CreateMap<GameScoreJsonModel, GameScore>()
            .ForMember(dest => dest.Ruleset, opt => opt.MapFrom(src => src.ModeInt))
            .ForMember(dest => dest.Mods, opt => opt.ConvertUsing(new ModsConverter()))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => ScoreGradeUtils.DetermineGrade(src)))
            .ForMember(dest => dest.SlotInfo, opt => opt.MapFrom(src => src.Match));

        CreateMap<BeatmapJsonModel, Beatmap>()
            .ForMember(dest => dest.StarRating, opt => opt.MapFrom(src => src.DifficultyRating))
            .ForMember(dest => dest.Ruleset, opt => opt.ConvertUsing(new RulesetConverter(), src => src.Mode))
            .ForMember(dest => dest.DifficultyName, opt => opt.MapFrom(src => src.Version));

        CreateMap<BeatmapExtendedJsonModel, BeatmapExtended>()
            .IncludeBase<BeatmapJsonModel, Beatmap>()
            .ForMember(dest => dest.HpDrain, opt => opt.MapFrom(src => src.Drain))
            .ForMember(dest => dest.OverallDifficulty, opt => opt.MapFrom(src => src.Accuracy));
    }
}
