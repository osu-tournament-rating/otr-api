using API.Services;

namespace API.Configurations;

public class DbCredentials : IDbCredentials
{
	public DbCredentials(string connectionString)
	{
		ConnectionString = connectionString;
	}
	
	public string ConnectionString { get; }
}