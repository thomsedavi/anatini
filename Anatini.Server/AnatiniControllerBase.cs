using System.Diagnostics;
using System.Security.Claims;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Mvc;
using User = Anatini.Server.Context.Entities.User;

namespace Anatini.Server
{
    public class AnatiniControllerBase : ControllerBase
    {
        public string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string RequiredUserId => UserId ?? throw new Exception();

        [NonAction]
        public async Task<IActionResult> UsingContextAsync(Func<AnatiniContext, Task<IActionResult>> contextFunction)
        {
            using var innerContext = new ContextBase();

            var result = await contextFunction(new AnatiniContext(innerContext));

            return result;
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelContext(string channelId, Func<Channel, AnatiniContext, IActionResult> channelContextFunction, bool requiresAccess = false) => await UsingChannelAsync(channelId, async channel =>
        {
            using var innerContext = new ContextBase();

            var result = channelContextFunction(channel, new AnatiniContext(innerContext));

            return await Task.FromResult(result);

        }, requiresAccess);

        [NonAction]
        public async Task<IActionResult> UsingChannelContextAsync(string channelId, Func<Channel, AnatiniContext, Task<IActionResult>> channelContextFunction, bool requiresAccess = false) => await UsingChannelAsync(channelId, async channel =>
        {
            using var innerContext = new ContextBase();

            return await channelContextFunction(channel, new AnatiniContext(innerContext));
        }, requiresAccess);

        [NonAction]
        public async Task<IActionResult> UsingContentContextAsync(string channelId, string contentId, Func<Content, Channel, AnatiniContext, Task<IActionResult>> contentContextFunction, string? eTag = null, bool refreshETag = false, bool requiresAccess = false) => await UsingContentAsync(channelId, contentId, async (content, channel) =>
        {
            using var innerContext = new ContextBase();

            return await contentContextFunction(content, channel, new AnatiniContext(innerContext));
        }, eTag, refreshETag, requiresAccess);

        [NonAction]
        public async Task<IActionResult> UsingContent(string channelId, string contentId, Func<Content, IActionResult> contentFunction, string? eTag = null, bool refreshETag = false, bool requiresAccess = false) => await UsingContentAsync(channelId, contentId, async (content, channel) =>
        {
            var result = contentFunction(content);

            return await Task.FromResult(result);
        }, eTag, refreshETag, requiresAccess);

        [NonAction]
        public async Task<IActionResult> UsingChannel(string channelId, Func<Channel, IActionResult> channelFunction, bool requiresAccess = false) => await UsingChannelAsync(channelId, async channel =>
        {
            var result = channelFunction(channel);

            return await Task.FromResult(result);
        }, requiresAccess);

        [NonAction]
        public async Task<IActionResult> UsingContentAsync(string channelId, string contentId, Func<Content, Channel, Task<IActionResult>> contentFunctionAsync, string? eTag = null, bool refreshETag = false, bool requiresAccess = false)
        {
            using var innerContext = new ContextBase();
            
            if (!RandomHex.IsX16(channelId))
            {
                var channelAlias = await innerContext.ChannelAliases.FindAsync(channelId);
            
                if (channelAlias == null)
                {
                    return NotFound();
                }
            
                channelId = channelAlias.ChannelId.ToString();
            }
            
            var channel = await innerContext.Channels.FindAsync(channelId);
            
            if (channel == null)
            {
                return Problem();
            }
            
            if (requiresAccess)
            {
                if (UserId == null || !channel.Users.Any(user => user.Id == UserId))
                {
                    return Forbid();
                }
            }
            
            if (!RandomHex.IsX16(contentId))
            {
                var contentAlias = await innerContext.ContentAliases.FindAsync(channelId, contentId);
            
                if (contentAlias == null)
                {
                    return NotFound();
                }
            
                contentId = contentAlias.ContentId.ToString();
            }
            
            var content = await innerContext.Contents.FindAsync(channelId, contentId);
            
            if (content == null)
            {
                return Problem();
            }
            
            if (eTag != null && eTag != content.ETag)
            {
                return ValidationProblem(statusCode: StatusCodes.Status412PreconditionFailed);
            }
            
            var result = await contentFunctionAsync(content, channel);
            
            if (refreshETag)
            {
                using var newInnerContext = new ContextBase();
            
                var newContent = await newInnerContext.Contents.FindAsync(channelId, contentId);
            
                Response.Headers.ETag = newContent?.ETag ?? Response.Headers.ETag;
            }
            else
            {
                Response.Headers.ETag = content.ETag;
            }
            
            return result;
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelAliasAsync(string channelSlug, Func<ChannelAlias, Task<IActionResult>> channelAliasFunctionAsync)
        {
            using var innerContext = new ContextBase();

            var channelAlias = await innerContext.ChannelAliases.FindAsync(channelSlug);

            if (channelAlias == null)
            {
                return NotFound();
            }

            return await channelAliasFunctionAsync(channelAlias);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserAliasAsync(string userSlug, Func<UserAlias, Task<IActionResult>> userAliasFunctionAsync)
        {
            using var innerContext = new ContextBase();

            var userAlias = await innerContext.UserAliases.FindAsync(userSlug);

            if (userAlias == null)
            {
                return NotFound();
            }

            return await userAliasFunctionAsync(userAlias);
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelAsync(string channelId, Func<Channel, Task<IActionResult>> channelFunctionAsync, bool requiresAccess = false)
        {

            using var innerContext = new ContextBase();

            if (!RandomHex.IsX16(channelId))
            {
                var channelAlias = await innerContext.ChannelAliases.FindAsync(channelId);

                if (channelAlias == null)
                {
                    return NotFound();
                }

                channelId = channelAlias.ChannelId.ToString();
            }

            var channel = await innerContext.Channels.FindAsync(channelId);

            if (channel == null)
            {
                return Problem();
            }

            if (requiresAccess)
            {
                if (UserId == null || !channel.Users.Any(user => user.Id == UserId))
                {
                    return Forbid();
                }
            }

            return await channelFunctionAsync(channel);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserContext(string userId, Func<User, AnatiniContext, IActionResult> userContextFunction, params UserPermission[] permissions) => await UsingUserAsync(userId, async user =>
        {
            using var innerContext = new ContextBase();

            var result = userContextFunction(user, new AnatiniContext(innerContext));

            return await Task.FromResult(result);
        }, permissions);

        [NonAction]
        public async Task<IActionResult> UsingUserContextAsync(string userId, Func<User, AnatiniContext, Task<IActionResult>> userContextFunction, params UserPermission[] permissions) => await UsingUserAsync(userId, async user =>
        {
            using var innerContext = new ContextBase();

            return await userContextFunction(user, new AnatiniContext(innerContext));
        }, permissions);

        [NonAction]
        public async Task<bool> UserHasAnyPermission(string? userId, params UserPermission[] permissions)
        {
            if (userId == null)
            {
                return false;
            }

            using var innerContext = new ContextBase();

            var user = await innerContext.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            return user.HasAnyPermission(permissions);
        }

        [NonAction]
        public async Task<IActionResult> UsingUserAsync(string userId, Func<User, Task<IActionResult>> userFunction, params UserPermission[] permissions)
        {
            using var innerContext = new ContextBase();

            var user = await innerContext.Users.FindAsync(userId);

            if (user == null)
            {
                return Problem();
            }

            if (permissions.Length > 0 && !user.HasAnyPermission(permissions))
            {
                return Forbid();
            }

            return await userFunction(user);
        }

        [NonAction]
        public bool ImageValidationError(CreateImage createImage, out ActionResult? result)
        {
            if (createImage.File == null || createImage.File.Length == 0)
            {
                result = BadRequest();
                return true;
            }

            if (!Enum.TryParse(createImage.Type, out ImageType imageType))
            {
                result = ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity);
                return true;
            }

            var extension = Path.GetExtension(createImage.File.FileName).ToLowerInvariant();

            if (extension != ".jpg" && extension != ".jpeg")
            {
                result = ValidationProblem(statusCode: StatusCodes.Status415UnsupportedMediaType);
                return true;
            }

            if (createImage.File.Length > 1024 * 1024)
            {
                result = ValidationProblem(statusCode: StatusCodes.Status413PayloadTooLarge);
                return true;
            }

            var (width, height) = imageType switch
            {
                ImageType.Banner => (1600, 900),
                ImageType.Card => (480, 360),
                ImageType.Icon => (400, 400),
                _ => throw new UnreachableException()
            };

            var dimensions = createImage.File.GetJpegDimensions();

            if (dimensions?.Width != width && dimensions?.Height != height)
            {
                result = ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity);
                return true;
            }

            result = null;
            return false;
        }
    }
}
