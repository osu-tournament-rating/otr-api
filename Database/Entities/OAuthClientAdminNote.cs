namespace Database.Entities;

public class OAuthClientAdminNote : AdminNoteEntityBase
{
    public OAuthClient OAuthClient { get; set; } = null!;
}
