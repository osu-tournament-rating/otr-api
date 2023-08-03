using Dapper;

namespace API.Entities;

[Table("config")]
public class Config
{
	public string Key { get; set; } = null!;
	public string Value { get; set; } = null!;
}