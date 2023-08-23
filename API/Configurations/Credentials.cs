namespace API.Configurations;

public class Credentials : ICredentials
{
	public Credentials(string connectionString, string? osuApiKey)
	{
		ConnectionString = connectionString;
		OsuApiKey = osuApiKey;
	}

	public string ConnectionString { get; }
	public string? OsuApiKey { get; }
}