using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Utilities.AdminNotes;

/// <summary>
/// Filters actions for only those that target a valid "entity" segment
/// </summary>
public class ValidateAdminNoteControllerRouteAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.RouteData.Values.TryGetValue("entity", out var value)
            && value is string
            && !AdminNotesHelper.GetAdminNoteableEntityRoutes().Contains(value)
           )
        {
            context.Result = new NotFoundResult();
        }
    }
}
