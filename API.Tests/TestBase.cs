using API.Configurations;
using AutoMapper;

namespace APITests;

public class TestBase
{
    protected static IMapper GetMapper()
    {
        var profile = new MapperProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
        return new Mapper(configuration);
    }
}
