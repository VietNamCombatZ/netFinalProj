using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            // Nếu chưa đăng nhập thì chuyển hướng về trang Login
            context.Result = new RedirectToActionResult("AccessDenied", "Users", null);
        }

        base.OnActionExecuting(context);
    }
}