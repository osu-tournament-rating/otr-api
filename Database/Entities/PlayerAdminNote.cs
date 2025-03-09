namespace Database.Entities;

public class PlayerAdminNote : AdminNoteEntityBase
{
    public Player Player { get; set; } = null!;
}
