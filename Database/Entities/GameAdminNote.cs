using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("game_admin_notes")]
public class GameAdminNote : AdminNoteEntityBase
{
    public Game Game { get; set; } = null!;
}
