using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace finalProject.Controllers.Filter
{
    public class RoleAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizationFilter(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.User?.FindFirst("Role")?.Value;

            if (string.IsNullOrEmpty(userRole) || userRole != _requiredRole)
            {
                // Nếu role không đúng, redirect về trang lỗi hoặc trang login
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    } 
    
}
