using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class PermissionFilter : ActionFilterAttribute
{
    private readonly string[] _allowedPermissions;

    public PermissionFilter(params string[] allowedPermissions)
    {
        _allowedPermissions = allowedPermissions;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Retrieve permission from session first
        var permissionPosition = context.HttpContext.Session.GetString("Permission_position");

        // If permission is not set in the session, check the user's permission from the model
        if (string.IsNullOrEmpty(permissionPosition) || !_allowedPermissions.Contains(permissionPosition))
        {
            // Clear the session before redirecting (if necessary)
            context.HttpContext.Session.Clear();

            // Redirect to login page
            context.Result = new RedirectToActionResult("LoginPage", "Home", null);
        }

        base.OnActionExecuting(context);
    }
}
