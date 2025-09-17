using System.Net;
using System.Security.Claims;
using Anatini.Server.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
{
    public class AnatiniControllerBase : ControllerBase
    {
        internal async Task<IActionResult> UsingUser(Func<User, Task<IActionResult>> userFunction)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var userResult = await new GetUser(userId).ExecuteAsync();

                if (userResult == null)
                {
                    return Problem();
                }

                var user = userResult!;

                try
                {
                    return await userFunction(user);
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                    {
                        return Conflict();
                    }
                    else
                    {
                        // add logger
                        return Problem();
                    }
                }

                catch (Exception)
                {
                    // add logger
                    return Problem();
                }
            }
            else
            {
                // add logging
                return Problem();
            }
        }
    }
}
