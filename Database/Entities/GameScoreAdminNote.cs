using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("game_score_admin_notes")]
public class GameScoreAdminNote : AdminNoteEntityBase
{
    public GameScore Score { get; set; } = null!;
}
