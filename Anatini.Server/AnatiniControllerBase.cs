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
        [NonAction]
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

        [NonAction]
        public async Task<IActionResult> UsingChannelContext(string slug, Func<Channel, AnatiniContext, IActionResult> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var context = new AnatiniContext();

                var result = channelContextFunction(channel, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingChannelAsync(slug, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelContextAsync(string slug, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var context = new AnatiniContext();

                var result = await channelContextFunction(channel, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingChannelAsync(slug, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingPost(string channelSlug, string postSlug, Func<Post, IActionResult> postFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> postFunctionAsync(Post post)
            {
                var result = postFunction(post);

                return await Task.FromResult(result);
            }

            return await UsingPostAsync(channelSlug, postSlug, postFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannel(string channelSlug, Func<Channel, IActionResult> channelFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                var result = channelFunction(channel);

                return await Task.FromResult(result);
            }

            return await UsingChannelAsync(channelSlug, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingPostAsync(string channelSlug, string postSlug, Func<Post, Task<IActionResult>> postFunctionAsync, bool _ = false)
        {
            try
            {
                using var context = new AnatiniContext();

                var channelAlias = await context.ChannelAliases.FindAsync(channelSlug);

                if (channelAlias == null)
                {
                    return NotFound();
                }

                var postAlias = await context.PostAliases.FindAsync(channelAlias.ChannelId, postSlug);

                if (postAlias == null)
                {
                    return NotFound();
                }

                var post = await context.Posts.FindAsync(channelAlias.ChannelId, postAlias.PostId);

                if (post == null)
                {
                    return Problem();
                }

                // add if requiresAuthorisation

                return await postFunctionAsync(post);
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

        [NonAction]
        public async Task<IActionResult> UsingChannelAsync(string channelSlug, Func<Channel, Task<IActionResult>> channelFunctionAsync, bool requiresAuthorisation = false)
        {

            try
            {
                using var context = new AnatiniContext();

                var channelAlias = await context.ChannelAliases.FindAsync(channelSlug);
                
                if (channelAlias == null)
                {
                    return NotFound();
                }

                var channel = await context.Channels.FindAsync(channelAlias.ChannelId);

                if (channel == null)
                {
                    return Problem();
                }

                if (requiresAuthorisation)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
                    if (!channel.Users.Any(user => user.Id == userId))
                    {
                        return Unauthorized();
                    }
                }

                return await channelFunctionAsync(channel);
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

        [NonAction]
        public async Task<IActionResult> UsingUserContext(Func<User, AnatiniContext,IActionResult> userContextFunction)
        {
            async Task<IActionResult> userFunctionAsync(User user)
            {
                using var context = new AnatiniContext();

                var result = userContextFunction(user, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingUserAsync(userFunctionAsync);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserContextAsync(Func<User, AnatiniContext, Task<IActionResult>> userContextFunction)
        {
            async Task<IActionResult> userFunctionAsync(User user)
            {
                using var context = new AnatiniContext();

                var result = await userContextFunction(user, context);

                await context.SaveChangesAsync();

                return result;
            }

            return await UsingUserAsync(userFunctionAsync);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserAsync(Func<User, Task<IActionResult>> userFunction)
        {
            try
            {
                using var context = new AnatiniContext();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = await context.Users.FindAsync(userId);

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

        [NonAction]
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
