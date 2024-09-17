using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("match_admin_notes")]
public class MatchAdminNote : AdminNoteEntityBase
{
    public Match Match { get; set; } = null!;
}
