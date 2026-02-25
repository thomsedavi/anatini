using Anatini.Server.Posts.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anatini.Server.Utils
{
    public class ETagRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var eTag = context.HttpContext.Request.ETagHeader();

            if (string.IsNullOrEmpty(eTag))
            {
                context.Result = new StatusCodeResult(428);

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
