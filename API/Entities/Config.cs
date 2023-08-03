using Dapper;

namespace API.Entities;

[Table("config")]
public class Config
{
	[Column("key")]
	public string Key { get; set; } = null!;
	[Column("value")]
	public string Value { get; set; } = null!;
}