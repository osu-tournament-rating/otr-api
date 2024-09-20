using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("tournament_admin_notes")]
public class TournamentAdminNote : AdminNoteEntityBase
{
    public Tournament Tournament { get; } = null!;
}
