using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("oauth_clients")]
public class OAuthClient
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [MaxLength(70)]
    [Column("secret")]
    public string Secret { get; set; } = string.Empty;

    [Column("scopes")]
    public string[] Scopes { get; set; } = [];

    [Column("user_id")]
    public int UserId { get; set; }

    [InverseProperty("Clients")]
    public virtual User User { get; set; } = null!;
}
