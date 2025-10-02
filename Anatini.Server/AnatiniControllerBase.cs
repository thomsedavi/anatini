using System.Net;
using System.Security.Claims;
using Anatini.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using User = Anatini.Server.Context.User;

namespace Anatini.Server
{
    public class AnatiniControllerBase : ControllerBase
    {
        public async Task<IActionResult> UsingContextAsync(Func<AnatiniContext, Task<IActionResult>> contextFunction, Func<DbUpdateException, IActionResult, IActionResult>? onDbUpdateException = null)
        {
            try
            {
                using var context = new AnatiniContext();

                var result = await contextFunction(context);

                await context.SaveChangesAsync();

                return result;
            }
            catch (DbUpdateException dbUpdateException)
            {
                return onDbUpdateException != null ? onDbUpdateException(dbUpdateException, DbUpdateExceptionResult(dbUpdateException)) : DbUpdateExceptionResult(dbUpdateException);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        public async Task<IActionResult> UsingChannelContext(string slug, bool requiresAuthorisation, Func<Channel, AnatiniContext, IActionResult> channelContextFunction)
        {
            async Task<IActionResult> channelFunction(Channel channel)
            {
                using var context = new AnatiniContext();

                var result = channelContextFunction(channel, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingChannel(slug, requiresAuthorisation, channelFunction);
        }

        public async Task<IActionResult> UsingChannelContextAsync(string slug, bool requiresAuthorisation, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction)
        {
            async Task<IActionResult> channelFunction(Channel channel)
            {
                using var context = new AnatiniContext();

                var result = await channelContextFunction(channel, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingChannel(slug, requiresAuthorisation, channelFunction);
        }

        public async Task<IActionResult> UsingChannel(string slug, bool requiresAuthorisation, Func<Channel, Task<IActionResult>> channelFunction)
        {

            try
            {
                using var context = new AnatiniContext();

                var channelAlias = await context.ChannelAliases.FindAsync(slug);
                
                if (channelAlias == null)
                {
                    return Problem();
                }

                var channel = await context.Channels.FindAsync(channelAlias.ChannelId);

                if (channel == null)
                {
                    return Problem();
                }

                if (requiresAuthorisation)
                {
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
                    if (!Guid.TryParse(userIdClaim, out var userId) || !channel.Users.Any(user => user.Id == userId))
                    {
                        return Unauthorized();
                    }
                }

                return await channelFunction(channel);
            }
            catch (DbUpdateException dbUpdateException)
            {
                return DbUpdateExceptionResult(dbUpdateException);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        public async Task<IActionResult> UsingUserContext(Func<User, AnatiniContext,IActionResult> userContextFunction)
        {
            async Task<IActionResult> userFunction(User user)
            {
                using var context = new AnatiniContext();

                var result = userContextFunction(user, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingUser(userFunction);
        }

        public async Task<IActionResult> UsingUserContextAsync(Func<User, AnatiniContext, Task<IActionResult>> userContextFunction)
        {
            async Task<IActionResult> userFunction(User user)
            {
                using var context = new AnatiniContext();

                var result = await userContextFunction(user, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingUser(userFunction);
        }

        public async Task<IActionResult> UsingUser(Func<User, Task<IActionResult>> userFunction)
        {
            try
            {
                using var context = new AnatiniContext();

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out var userGuid))
                {
                    return Problem();
                }

                var user = await context.Users.FindAsync(userGuid);

                if (user == null)
                {
                    return Problem();
                }

                return await userFunction(user);
            }
            catch (DbUpdateException dbUpdateException)
            {
                return DbUpdateExceptionResult(dbUpdateException);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        private IActionResult DbUpdateExceptionResult(DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict();
            }
            else
            {
                return Problem();
            }
        }
    }
}
