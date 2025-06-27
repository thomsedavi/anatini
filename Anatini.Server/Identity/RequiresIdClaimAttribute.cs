using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace anatini.Server.Identity
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresIdClaimAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var actionId = context.RouteData.Values["id"]?.ToString();

            if (actionId == null)
            {
                context.Result = new BadRequestResult();
                return;
            }

            // check user can access this item
            var isAuthorized = await Task.FromResult<bool>(context.HttpContext.User.HasClaim(ClaimTypes.NameIdentifier, actionId));

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
