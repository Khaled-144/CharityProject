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
        // Retrieve permission from session
        var permissionPosition = context.HttpContext.Session.GetString("Permission_position");

        // If session is empty, redirect to login page
        if (string.IsNullOrEmpty(permissionPosition))
        {
            context.Result = new RedirectToActionResult("LoginPage", "Home", null);
            return; // Stop further execution
        }

        // Check if the user's permission is allowed
        if (!_allowedPermissions.Contains(permissionPosition))
        {
            context.Result = new RedirectToActionResult("NotAuthorized", "Home", null);
            return; // Stop further execution
        }

        // Call base method if authorization is successful
        base.OnActionExecuting(context);
    }

}
