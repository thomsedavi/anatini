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
                var channelAliasResult = await new GetChannelAlias(slug).ExecuteAsync();
                
                if (channelAliasResult == null)
                {
                    return Problem();
                }

                var channelAlias = channelAliasResult!;

                var channelResult = await new GetChannel(channelAlias.ChannelGuid).ExecuteAsync();

                if (channelResult == null)
                {
                    return Problem();
                }

                var channel = channelResult!;

                if (requiresAuthorisation)
                {
                    var userGuidClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
                    if (!Guid.TryParse(userGuidClaim, out var userGuid) || !channel.Users.Any(user => user.Guid == userGuid))
                    {
                        return Unauthorized();
                    }
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

                var userGuidClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userGuidClaim, out var userGuid))
                {
                    // add logging
                    return Problem();
                }

                var userResult = await new GetUser(userGuid).ExecuteAsync();

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
