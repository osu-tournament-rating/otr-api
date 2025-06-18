using JetBrains.Annotations;

namespace Database.Entities;


[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TournamentAdminNote : AdminNoteEntityBase
{
    public Tournament Tournament { get; } = null!;
}
