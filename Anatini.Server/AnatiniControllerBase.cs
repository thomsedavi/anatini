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
        public async Task<IActionResult> UsingChannelContext(string channelId, Func<Channel, AnatiniContext, IActionResult> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var innerContext = new ContextBase();

                var result = channelContextFunction(channel, new AnatiniContext(innerContext));

                return await Task.FromResult(result);
            }

            return await UsingChannelAsync(channelId, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelContextAsync(string channelId, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                using var innerContext = new ContextBase();

                return await channelContextFunction(channel, new AnatiniContext(innerContext));
            }

            return await UsingChannelAsync(channelId, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingContentContextAsync(string channelId, string contentId, Func<Content, AnatiniContext, Task<IActionResult>> contentContextFunction, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> contentFunctionAsync(Content content)
            {
                using var innerContext = new ContextBase();

                return await contentContextFunction(content, new AnatiniContext(innerContext));
            }

            return await UsingContentAsync(channelId, contentId, contentFunctionAsync, eTag, refreshETag, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingContent(string channelId, string contentId, Func<Content, IActionResult> contentFunction, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> contentFunctionAsync(Content content)
            {
                var result = contentFunction(content);

                return await Task.FromResult(result);
            }

            return await UsingContentAsync(channelId, contentId, contentFunctionAsync, eTag, refreshETag, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannel(string channelId, Func<Channel, IActionResult> channelFunction, bool requiresAuthorisation = false)
        {
            async Task<IActionResult> channelFunctionAsync(Channel channel)
            {
                var result = channelFunction(channel);

                return await Task.FromResult(result);
            }

            return await UsingChannelAsync(channelId, channelFunctionAsync, requiresAuthorisation);
        }

        [NonAction]
        public async Task<IActionResult> UsingContentAsync(string channelId, string contentId, Func<Content, Task<IActionResult>> contentFunctionAsync, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false)
        {
            try
            {
                using var innerContext = new ContextBase();

                if (!Guid.TryParse(channelId, out Guid _))
                {
                    var channelAlias = await innerContext.ChannelAliases.FindAsync(channelId);

                    if (channelAlias == null)
                    {
                        return NotFound();
                    }

                    channelId = channelAlias.ChannelId.ToString();
                }

                if (requiresAuthorisation)
                {
                    var channel = await innerContext.Channels.FindAsync(new Guid(channelId));

                    if (channel == null)
                    {
                        return Problem();
                    }

                    if (!channel.Users.Any(user => user.Id == UserId))
                    {
                        return Forbid();
                    }
                }

                if (!Guid.TryParse(contentId, out Guid _))
                {
                    var contentAlias = await innerContext.ContentAliases.FindAsync(new Guid(channelId), contentId);

                    if (contentAlias == null)
                    {
                        return NotFound();
                    }

                    contentId = contentAlias.ContentId.ToString();
                }

                var content = await innerContext.Contents.FindAsync(new Guid(channelId), new Guid(contentId));

                if (content == null)
                {
                    return Problem();
                }

                if (eTag != null && eTag != content.ETag)
                {
                    return ValidationProblem(statusCode: 412);
                }

                var result = await contentFunctionAsync(content);

                if (refreshETag)
                {
                    using var newInnerContext = new ContextBase();

                    var newContent = await newInnerContext.Contents.FindAsync(new Guid(channelId), new Guid(contentId));

                    Response.Headers.ETag = newContent!.ETag;
                }
                else
                {
                    Response.Headers.ETag = content.ETag;
                }

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
        public async Task<IActionResult> UsingChannelAsync(string channelId, Func<Channel, Task<IActionResult>> channelFunctionAsync, bool requiresAuthorisation = false)
        {

            try
            {
                using var innerContext = new ContextBase();

                if (!Guid.TryParse(channelId, out Guid _))
                {
                    var channelAlias = await innerContext.ChannelAliases.FindAsync(channelId);

                    if (channelAlias == null)
                    {
                        return NotFound();
                    }

                    channelId = channelAlias.ChannelId.ToString();
                }

                var channel = await innerContext.Channels.FindAsync(new Guid(channelId));

                if (channel == null)
                {
                    return Problem();
                }

                if (requiresAuthorisation)
                {
                    if (!channel.Users.Any(user => user.Id == UserId))
                    {
                        return Forbid();
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
