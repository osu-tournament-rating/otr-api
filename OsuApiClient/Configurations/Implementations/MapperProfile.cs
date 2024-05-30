using AutoMapper;
using OsuApiClient.Domain.Site;
using OsuApiClient.Domain.Users;
using OsuApiClient.Domain.Users.Attributes;
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
        CreateMap<UserExtendedJsonModel, UserExtended>();

        CreateMap<GroupJsonModel, Group>();
        CreateMap<UserGroupJsonModel, UserGroup>();
    }
}
