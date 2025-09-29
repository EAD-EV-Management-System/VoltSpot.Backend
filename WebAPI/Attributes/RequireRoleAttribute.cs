using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebAPI.Attributes
{
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public RequireRoleAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRole) || userRole != _requiredRole)
            {
                context.Result = new ObjectResult(new
                {
                    Success = false,
                    Message = $"Access denied. {_requiredRole} role required."
                })
                {
                    StatusCode = 403
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
