using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Utilities.AdminNotes;

public class AdminNoteRouteTarget : IParsable<AdminNoteRouteTarget>
{
    [SwaggerIgnore]
    public string EntityName { get; init; } = string.Empty;

    [SwaggerIgnore]
    public Type? EntityType { get; init; }

    [SwaggerIgnore]
    public Type? AdminNoteType { get; init; }

    public static AdminNoteRouteTarget Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out AdminNoteRouteTarget result))
        {
            throw new ArgumentException("Could not parse admin note route target", nameof(s));
        }

        return result;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out AdminNoteRouteTarget result
    )
    {
        result = null;
        return false;
    }
}
