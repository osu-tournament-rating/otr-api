using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("player_admin_notes")]
public class PlayerAdminNote : AdminNoteEntityBase
{
    public Player Player { get; set; } = null!;
}
