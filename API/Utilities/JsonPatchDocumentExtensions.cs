using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace API.Utilities;

public static class JsonPatchDocumentExtensions
{
    /// <summary>
    /// Denotes the patch as having only operations of type Replace
    /// </summary>
    /// <param name="patchDocument"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsReplaceOnly<T>(this JsonPatchDocument<T> patchDocument) where T : class
    {
        return patchDocument.Operations.All(op => op.OperationType == OperationType.Replace);
    }

    /// <summary>
    /// Returns any provided fields that appear in any operation's path property
    /// </summary>
    /// <param name="patchDocument"></param>
    /// <param name="fields"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<string> IncludesFields<T>(this JsonPatchDocument<T> patchDocument, IEnumerable<string> fields)
        where T : class
    {
        IEnumerable<string> normalizedFields = fields.Select(f => '/' + f);
        return patchDocument.Operations
            .Where(op => normalizedFields.Any(f => string.Equals(f, op.path, StringComparison.OrdinalIgnoreCase)))
            .Select(op => op.path.TrimStart('/'))
            .Distinct();
    }
}
