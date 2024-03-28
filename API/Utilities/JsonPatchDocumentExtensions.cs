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
}
