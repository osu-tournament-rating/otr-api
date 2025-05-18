namespace Database.Entities;

public class MatchAdminNote : AdminNoteEntityBase
{
    public Match Match { get; set; } = null!;
}
