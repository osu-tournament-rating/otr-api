using Dapper;
#pragma warning disable CS8618

namespace API.Entities;

[Table("config")]
public class Config
{
	[Column("key")]
	public string Key { get; set; }
	[Column("value")]
	public string Value { get; set; }
}