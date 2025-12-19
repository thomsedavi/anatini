using System.Net;
using System.Security.Claims;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using User = Anatini.Server.Context.Entities.User;

namespace Anatini.Server
{
    public class AnatiniControllerBase : ControllerBase
    {
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
            catch (Exception exception)
            {
                return ExceptionResult(exception);
            }
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelContext(string channelId, Func<Channel, AnatiniContext, IActionResult> channelContextFunction, bool requiresAuthorisation = false) => await UsingChannelAsync(channelId, async channel =>
        {
            using var innerContext = new ContextBase();

            var result = channelContextFunction(channel, new AnatiniContext(innerContext));

            return await Task.FromResult(result);

        }, requiresAuthorisation);

        [NonAction]
        public async Task<IActionResult> UsingChannelContextAsync(string channelId, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction, bool requiresAuthorisation = false) => await UsingChannelAsync(channelId, async channel =>
        {
            using var innerContext = new ContextBase();

            return await channelContextFunction(channel, new AnatiniContext(innerContext));
        }, requiresAuthorisation);

        [NonAction]
        public async Task<IActionResult> UsingContentContextAsync(string channelId, string contentId, Func<Content, Channel, AnatiniContext, Task<IActionResult>> contentContextFunction, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false) => await UsingContentAsync(channelId, contentId, async (content, channel) =>
        {
            using var innerContext = new ContextBase();

            return await contentContextFunction(content, channel, new AnatiniContext(innerContext));
        }, eTag, refreshETag, requiresAuthorisation);

        [NonAction]
        public async Task<IActionResult> UsingContent(string channelId, string contentId, Func<Content, IActionResult> contentFunction, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false) => await UsingContentAsync(channelId, contentId, async (content, channel) =>
        {
            var result = contentFunction(content);

            return await Task.FromResult(result);
        }, eTag, refreshETag, requiresAuthorisation);

        [NonAction]
        public async Task<IActionResult> UsingChannel(string channelId, Func<Channel, IActionResult> channelFunction, bool requiresAuthorisation = false) => await UsingChannelAsync(channelId, async channel =>
        {
            var result = channelFunction(channel);

            return await Task.FromResult(result);
        }, requiresAuthorisation);

        [NonAction]
        public async Task<IActionResult> UsingContentAsync(string channelId, string contentId, Func<Content, Channel, Task<IActionResult>> contentFunctionAsync, string? eTag = null, bool refreshETag = false, bool requiresAuthorisation = false)
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

                var result = await contentFunctionAsync(content, channel);

                if (refreshETag)
                {
                    using var newInnerContext = new ContextBase();

                    var newContent = await newInnerContext.Contents.FindAsync(new Guid(channelId), new Guid(contentId));

                    Response.Headers.ETag = newContent?.ETag ?? Response.Headers.ETag;
                }
                else
                {
                    Response.Headers.ETag = content.ETag;
                }

                return result;
            }
            catch (Exception exception)
            {
                return ExceptionResult(exception);
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
            catch (Exception exception)
            {
                return ExceptionResult(exception);
            }
        }

        [NonAction]
        public async Task<IActionResult> UsingUserContext(Guid userId, Func<User, AnatiniContext, IActionResult> userContextFunction) => await UsingUserAsync(userId, async user =>
        {
            using var innerContext = new ContextBase();

            var result = userContextFunction(user, new AnatiniContext(innerContext));

            return await Task.FromResult(result);
        });

        [NonAction]
        public async Task<IActionResult> UsingUserContextAsync(Guid userId, Func<User, AnatiniContext, Task<IActionResult>> userContextFunction) => await UsingUserAsync(userId, async user =>
        {
            using var innerContext = new ContextBase();

            return await userContextFunction(user, new AnatiniContext(innerContext));
        });

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
            catch (Exception exception)
            {
                return ExceptionResult(exception);
            }
        }

        [NonAction]
        public IActionResult ExceptionResult(Exception exception)
        {
            if (exception is KeyNotFoundException)
            {
                return NotFound();
            }
            else if (exception is DbUpdateException dbUpdateException && dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
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
