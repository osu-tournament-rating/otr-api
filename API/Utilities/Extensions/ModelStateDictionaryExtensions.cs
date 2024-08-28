using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Utilities.Extensions;

public static class ModelStateDictionaryExtensions
{
    /// <summary>
    /// Formats all errors in the model state into a single string
    /// </summary>
    public static string ErrorMessage(this ModelStateDictionary modelState) =>
        string.Join(", ", modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
}
