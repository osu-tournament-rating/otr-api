using Database.Enums;

namespace API.DTOs;

public class GameScoreDTO
{
    public int PlayerId { get; set; }
    public Team Team { get; set; }
    public int Score { get; set; }
    public Mods Mods { get; set; }
    public int Misses { get; set; }
    public double Accuracy { get; set; }
}
