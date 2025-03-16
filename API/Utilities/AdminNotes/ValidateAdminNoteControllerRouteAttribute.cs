using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Utilities.AdminNotes;

public class ValidateAdminNoteControllerRouteAttribute : ActionFilterAttribute
{
    private static readonly string[] s_validRoutes = AdminNotesHelper
        .GetAdminNoteableEntityTypes()
        .Select(t => JsonNamingPolicy.CamelCase.ConvertName(t.Name))
        .ToArray();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.RouteData.Values.TryGetValue("entity", out var value)
            && value is string
            && !s_validRoutes.Contains(value)
           )
        {
            context.Result = new NotFoundResult();
        }
    }
}
