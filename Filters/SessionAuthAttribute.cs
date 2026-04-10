using HospitalEHR.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalEHR.Filters
{
    /// <summary>
    /// Replaces [Authorize]. Checks ISession for authentication + role.
    /// Usage: [SessionAuth] or [SessionAuth("Admin","Doctor")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class SessionAuthAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;
        public SessionAuthAttribute(params string[] roles) { _roles = roles; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            if (!session.IsAuthenticated())
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            if (_roles.Length > 0 && !session.IsInRoles(_roles))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
