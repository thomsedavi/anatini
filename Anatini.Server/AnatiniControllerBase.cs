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
        public string? CookieValue(string key) => Request.Cookies.FirstOrDefault(cookie => cookie.Key == key).Value;
        public void DeleteCookie(string key) => Response.Cookies.Delete(key);
        public Guid UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId) ? userId : Guid.Empty;

        [NonAction]
        public async Task<IActionResult> UsingContextAsync(Func<AnatiniContext, Task<IActionResult>> contextFunction)
        {
            try
            {
                using var innerContext = new ContextBase();

                var result = await contextFunction(new AnatiniContext(innerContext));

                return result;
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
        public async Task<IActionResult> UsingChannelContext(string slug, Func<Channel, AnatiniContext, IActionResult> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var innerContext = new ContextBase();

                var result = channelContextFunction(channel, new AnatiniContext(innerContext));

                return await Task.FromResult(result);
            }

            return await UsingChannelAsync(slug, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelContextAsync(string slug, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var innerContext = new ContextBase();

                return await channelContextFunction(channel, new AnatiniContext(innerContext));
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
                using var innerContext = new ContextBase();

                var channelAlias = await innerContext.ChannelAliases.FindAsync(channelSlug);

                if (channelAlias == null)
                {
                    return NotFound();
                }

                var postAlias = await innerContext.PostAliases.FindAsync(channelAlias.ChannelId, postSlug);

                if (postAlias == null)
                {
                    return NotFound();
                }

                var post = await innerContext.Posts.FindAsync(channelAlias.ChannelId, postAlias.PostId);

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
                using var innerContext = new ContextBase();

                var channelAlias = await innerContext.ChannelAliases.FindAsync(channelSlug);
                
                if (channelAlias == null)
                {
                    return NotFound();
                }

                var channel = await innerContext.Channels.FindAsync(channelAlias.ChannelId);

                if (channel == null)
                {
                    return Problem();
                }

                if (requiresAuthorisation)
                {
                    if (!channel.Users.Any(user => user.Id == UserId))
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
        public async Task<IActionResult> UsingUserContext(Guid userId, Func<User, AnatiniContext, IActionResult> userContextFunction)
        {
            async Task<IActionResult> userFunctionAsync(User user)
            {
                using var innerContext = new ContextBase();

                var result = userContextFunction(user, new AnatiniContext(innerContext));

                return await Task.FromResult(result);
            }

            return await UsingUserAsync(userId, userFunctionAsync);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserContextAsync(Guid userId, Func<User, AnatiniContext, Task<IActionResult>> userContextFunction)
        {
            async Task<IActionResult> userFunctionAsync(User user)
            {
                using var innerContext = new ContextBase();

                return await userContextFunction(user, new AnatiniContext(innerContext));
            }

            return await UsingUserAsync(userId, userFunctionAsync);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserAsync(Guid userId, Func<User, Task<IActionResult>> userFunction)
        {
            try
            {
                using var innerContext = new ContextBase();

                var user = await innerContext.Users.FindAsync(userId);

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
