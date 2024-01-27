using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Keyless]
[Table("logs")]
public class Log
{
	[Column("message")]
	public string? Message { get; set; }
	[Column("message_template")]
	public string? MessageTemplate { get; set; }
	[Column("level")]
	public int? Level { get; set; }
	[Column("timestamp", TypeName = "timestamp with time zone")]
	public DateTime? Timestamp { get; set; }
	[Column("exception")]
	public string? Exception { get; set; }
	[Column("log_event", TypeName = "jsonb")]
	public string? LogEvent { get; set; }
}