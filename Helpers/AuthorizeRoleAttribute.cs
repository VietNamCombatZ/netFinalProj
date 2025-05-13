using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace finalProject.Helpers;

// Helpers/AuthorizeRoleAttribute.cs


public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string _requiredRole;

    public AuthorizeRoleAttribute(string requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var role = context.HttpContext.Request.Cookies["UserRole"];
        if (string.IsNullOrEmpty(role) || role != _requiredRole)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Users", null);
        }
    }
}
