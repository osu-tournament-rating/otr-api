using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities;

[Table("oauth_client_admin_notes")]
public class OAuthClientAdminNote : AdminNoteEntityBase
{
    public OAuthClient OAuthClient { get; set; } = null!;
}
