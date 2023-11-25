namespace API.Configurations;

public interface ICredentials
{
	/// <summary>
	///  Connection string to PostgreSQL database
	/// </summary>
	string ConnectionString { get; }
	/// <summary>
	///  osu! API v1 key
	/// </summary>
	string? OsuApiKey { get; }
}