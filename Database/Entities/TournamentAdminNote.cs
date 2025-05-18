namespace Database.Entities;

public class TournamentAdminNote : AdminNoteEntityBase
{
    public Tournament Tournament { get; } = null!;
}
