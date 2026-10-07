using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anatini.Server.Utils
{
    public class DisableEndpointAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
        }
    }
}
