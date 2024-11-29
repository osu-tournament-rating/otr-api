namespace API.SwaggerGen;

/// <summary>
/// Contains values of keys used for custom <see cref="Microsoft.OpenApi.Interfaces.IOpenApiExtension"/>s
/// </summary>
public static class ExtensionKeys
{
    /// <summary>
    /// Extension containing a list of enum names
    /// </summary>
    public const string EnumNames = "x-enumNames";

    /// <summary>
    /// Extension containing a list of enum descriptions
    /// </summary>
    public const string EnumDescriptions = "x-enumDescriptions";

    /// <summary>
    /// Extension denoting an enum is a bitwise flag
    /// </summary>
    public const string EnumBitwiseFlag = "x-bitwiseFlag";

    /// <summary>
    /// Extension denoting an operation requires authorization
    /// </summary>
    public const string OperationRequiresAuthorization = "x-requiresAuthorization";
}
