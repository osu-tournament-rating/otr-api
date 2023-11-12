using API.Configurations;
using AutoMapper;

namespace APITests;

public static class ConfigurationInstances
{
	public static IMapper Mapper => CreateMapper();
	
	
	private static IMapper CreateMapper() 
	{
		var config = new MapperConfiguration(cfg => { cfg.AddProfile<MapperProfile>(); });
		return config.CreateMapper();
	}
}