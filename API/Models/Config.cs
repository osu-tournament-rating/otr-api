using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Keyless]
[Table("config")]
public partial class Config
{
    [Column("key")]
    public string Key { get; set; } = null!;

    [Column("value")]
    public string Value { get; set; } = null!;

    [Column("id")]
    public int Id { get; set; }

    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime? Created { get; set; }
}
