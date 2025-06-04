using System.ComponentModel;
using System.Reflection;

namespace OsuApiClient.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the value of the <see cref="DescriptionAttribute"/> for the given enum
    /// </summary>
    /// <returns>The enum description, or an empty string if one is not found</returns>
    public static string GetDescription(this Enum value)
    {
        Type type = value.GetType();
        string? name = type.GetEnumName(value);

        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        FieldInfo? field = type.GetField(name);
        if (field is null)
        {
            return string.Empty;
        }

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
        {
            return attr.Description;
        }

        return string.Empty;
    }
}
