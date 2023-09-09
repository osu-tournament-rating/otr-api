using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Table("ratings")]
[Index("PlayerId", "Mode", Name = "ratings_playerid_mode", IsUnique = true)]
public partial class Rating
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [Column("mu")]
    public double Mu { get; set; }

    [Column("sigma")]
    public double Sigma { get; set; }

    [Column("created", TypeName = "timestamp with time zone")]
    public DateTime Created { get; set; }

    [Column("updated", TypeName = "timestamp with time zone")]
    public DateTime? Updated { get; set; }

    [Column("mode")]
    public int Mode { get; set; }

    [ForeignKey("PlayerId")]
    [InverseProperty("Ratings")]
    public virtual Player Player { get; set; } = null!;
}
