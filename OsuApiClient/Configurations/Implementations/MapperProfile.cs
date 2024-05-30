using AutoMapper;
using OsuApiClient.Domain.Site;
using OsuApiClient.Domain.Users;
using OsuApiClient.Domain.Users.Attributes;
using OsuApiClient.Net.Deserialization.ValueConverters;
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
            .ForMember(dest => dest.Rulesets, opt => opt.MapFrom(src => src.PlayModes != null ? src.PlayModes.Select(RulesetConverter.Convert) : null));
    }
}
