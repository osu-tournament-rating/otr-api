using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("ratinghistories")]
[Index("PlayerId", "GameId", Name = "ratinghistories_pk", IsUnique = true)]
public partial class RatingHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [Column("mu")]
    public double Mu { get; set; }

    [Column("sigma")]
    public double Sigma { get; set; }

    [Column("created", TypeName = "timestamp without time zone")]
    public DateTime Created { get; set; }

    [Column("mode")]
    public int Mode { get; set; }

    [Column("game_id")]
    public int GameId { get; set; }

    [Column("updated", TypeName = "timestamp without time zone")]
    public DateTime? Updated { get; set; }

    [ForeignKey("GameId")]
    [InverseProperty("Ratinghistories")]
    public virtual Match Game { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("Ratinghistories")]
    public virtual Player Player { get; set; } = null!;
}
