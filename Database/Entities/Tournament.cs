using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("tournaments")]
public class Tournament : TournamentEntityBase
{
    [InverseProperty("Tournament")]
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
