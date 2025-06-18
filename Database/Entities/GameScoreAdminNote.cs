namespace Database.Entities;

public class GameScoreAdminNote : AdminNoteEntityBase
{
    public GameScore Score { get; init; } = null!;
}
