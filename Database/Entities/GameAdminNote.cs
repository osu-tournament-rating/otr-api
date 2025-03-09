namespace Database.Entities;

public class GameAdminNote : AdminNoteEntityBase
{
    public Game Game { get; set; } = null!;
}
