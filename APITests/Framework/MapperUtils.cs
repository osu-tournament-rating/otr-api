using API.Configurations;
using AutoMapper;

namespace APITests.Framework;

public class MapperUtils
{
    public static IMapper Instance()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MapperProfile>();
        });

        return config.CreateMapper();
    }
}
