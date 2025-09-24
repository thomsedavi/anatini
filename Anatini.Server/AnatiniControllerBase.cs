using System.Net;
using System.Security.Claims;
using Anatini.Server.Channels.Queries;
using Anatini.Server.Context;
using Anatini.Server.Users.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using User = Anatini.Server.Context.User;

namespace Anatini.Server
{
    public class AnatiniControllerBase : ControllerBase
    {
        public async Task<IActionResult> UsingChannel(string slug, bool requiresAuthorisation, Func<Channel, Task<IActionResult>> channelFunction)
        {

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    // add logging
                    return Problem();
                }

                var channelSlugResult = await new GetChannelSlug(slug).ExecuteAsync();
                
                if (channelSlugResult == null)
                {
                    return Problem();
                }

                var channelSlug = channelSlugResult!;

                var channelResult = await new GetChannel(channelSlug.ChannelId).ExecuteAsync();

                if (channelResult == null)
                {
                    return Problem();
                }

                var channel = channelResult!;
                
                if (requiresAuthorisation && !channel.Users.Any(user => user.Id == userId))
                {
                    return Unauthorized();
                }

                return await channelFunction(channel);
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

        public async Task<IActionResult> UsingUser(Func<User, Task<IActionResult>> userFunction)
        {
            try
            {

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    // add logging
                    return Problem();
                }

                var userResult = await new GetUser(userId).ExecuteAsync();

                if (userResult == null)
                {
                    return Problem();
                }

                var user = userResult!;

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
    }
}
