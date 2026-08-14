using System.Security.Claims;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnreachableException = System.Diagnostics.UnreachableException;

namespace Anatini.Server
{
    public class AnatiniControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : ControllerBase
    {
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        public bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        public string NormalizeHandle(string handle) => handle.ToLower();
        public string? NormalizeHandleOrNull(string? handle) => handle != null ? NormalizeHandle(handle) : null;
        public string NormalizeName(string name) => userManager.NormalizeName(name);
        public string NormalizeEmail(string email) => userManager.NormalizeEmail(email);
        private IActionResult CannotReadResponse() => IsAuthenticated ? Forbid() : Unauthorized();

        public ApplicationDbContext Context => context;
        public UserManager<ApplicationUser> UserManager => userManager;
        public IBlobService BlobService => blobService;
        private IAuthorizationService AuthorizationService => HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        [NonAction]
        public async Task<IActionResult> UsingAccountAsync(Func<ApplicationUser, Task<IActionResult>> accountFunction, ContextSettings? settings = null)
        {
            var usersQuery = context.Users.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                usersQuery = usersQuery.AsNoTracking();
            }

            usersQuery = usersQuery
                .Include(user => user.Images)
                .Include(user => user.SpaceEdges.Where(userSpaceEdge => userSpaceEdge.Label == UserSpaceEdgeLabel.Owner)).ThenInclude(userSpaceEdge => userSpaceEdge.TargetSpace);

            if (TryGetUserId(out Guid userId))
            {
                var user = await usersQuery.FirstOrDefaultAsync(user => user.Id == userId);

                if (user == null)
                {
                    return Problem();
                }

                return await accountFunction(user);
            }

            return Unauthorized();
        }

        [NonAction]
        public async Task<IActionResult> UsingUserAsync(string userHandle, Func<ApplicationUser, Task<IActionResult>> userFunction, ContextSettings? settings = null)
        {
            ApplicationUser? userResult;

            var usersQuery = context.Users.AsQueryable();

            if (settings != null)
            {
                if (settings.AsNoTracking)
                {
                    usersQuery = usersQuery.AsNoTracking();
                }

                if (settings.IncludeImages)
                {
                    usersQuery = usersQuery.Include((user) => user.Images);
                }
            }

            if (TryGetUserId(out Guid sourceUserId))
            {
                usersQuery = usersQuery.Include(user => user.ReceivedUserEdges.Where(userUserEdge => userUserEdge.SourceUserId == sourceUserId));
            }

            if (Guid.TryParse(userHandle, out Guid userId))
            {
                userResult = await usersQuery.FirstOrDefaultAsync(user => user.Id == userId);
            }
            else
            {
                var normalizedUserHandle = NormalizeHandle(userHandle);

                userResult = await usersQuery.FirstOrDefaultAsync(user => user.Handle == normalizedUserHandle || user.Handles.Any(handle => handle.Handle == normalizedUserHandle));
            }

            if (userResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(userResult.Visibility))
            {
                return await userFunction(userResult);
            }

            return CannotReadResponse();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpaceAsync(string spaceHandle, Func<Space, Task<IActionResult>> spaceFunction, ContextSettings? settings = null)
        {
            Space? spaceResult;

            var spacesQuery = context.Spaces.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                spacesQuery = spacesQuery.AsNoTracking();
            }

            spacesQuery = spacesQuery.Include(space => space.Images);

            if (Guid.TryParse(spaceHandle, out Guid spaceId))
            {
                spaceResult = await spacesQuery.FirstOrDefaultAsync(space => space.Id == spaceId);
            }
            else
            {
                var normalizedSpaceHandle = NormalizeHandle(spaceHandle);

                spaceResult = await spacesQuery.FirstOrDefaultAsync(space => space.Handle == normalizedSpaceHandle || space.Handles.Any(handle => handle.Handle == normalizedSpaceHandle));
            }

            if (spaceResult == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (await CanWriteSpaceAsync(spaceResult))
                {
                    return await spaceFunction(spaceResult);
                }

                return CannotReadResponse();
            }

            if (await CanReadAsync(spaceResult.Visibility))
            {
                return await spaceFunction(spaceResult);
            }

            return CannotReadResponse();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpacePostAsync(string spaceHandle, string postHandle, PostType postType, Func<Post, Task<IActionResult>> postFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            Post? postResult;

            var postsQuery = context.Posts.Where(post => post.Type == postType);

            if (settings?.AsNoTracking ?? true)
            {
                postsQuery = postsQuery.AsNoTracking();
            }

            if (Guid.TryParse(postHandle, out Guid postId))
            {
                postResult = await postsQuery.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Id == postId);
            }
            else
            {
                var normalizedPostHandle = NormalizeHandle(postHandle);

                postResult = await postsQuery.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Handle == normalizedPostHandle);
            }

            if (postResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(postResult.Visibility))
            {
                return await postFunction(postResult);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserPostAsync(string userHandle, string postHandle, PostType postType, Func<Post, Task<IActionResult>> postFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            Post? postResult;

            var postsQuery = context.Posts.Where(post => post.Type == postType);

            if (settings?.AsNoTracking ?? true)
            {
                postsQuery = postsQuery.AsNoTracking();
            }

            if (Guid.TryParse(postHandle, out Guid postId))
            {
                postResult = await postsQuery.FirstOrDefaultAsync(post => post.UserId == user.Id && post.Id == postId);
            }
            else
            {
                var normalizedPostHandle = NormalizeHandle(postHandle);

                postResult = await postsQuery.FirstOrDefaultAsync(post => post.UserId == user.Id && post.Handle == normalizedPostHandle);
            }

            if (postResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(postResult.Visibility))
            {
                return await postFunction(postResult);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserEventInstanceAsync(string userHandle, string eventSeriesHandle, string eventInstanceHandle, Func<EventInstance, Task<IActionResult>> eventInstanceFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            EventInstance? eventInstanceResult;

            var eventInstancesQuery = context.EventInstances.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                eventInstancesQuery = eventInstancesQuery.AsNoTracking();
            }

            if (TryGetUserId(out Guid userId))
            {
                eventInstancesQuery = eventInstancesQuery.Include(eventInstance => eventInstance.UserEdges.Where(userNote => userNote.SourceUserId == userId));
            }

            if (!Guid.TryParse(eventSeriesHandle, out Guid eventSeriesId))
            {
                var normalizedEventSeriesHandle = NormalizeHandle(eventSeriesHandle);

                var eventSeries = await context.EventSeries.FirstOrDefaultAsync(eventSeries => eventSeries.UserId == user.Id && eventSeries.Handle == normalizedEventSeriesHandle);

                if (eventSeries == null)
                {
                    return NotFound();
                }

                eventSeriesId = eventSeries.Id;
            }

            if (Guid.TryParse(eventInstanceHandle, out Guid eventInstanceId))
            {
                eventInstanceResult = await eventInstancesQuery.FirstOrDefaultAsync(eventInstance => eventInstance.UserId == user.Id && eventInstance.EventSeriesId == eventSeriesId && eventInstance.Id == eventInstanceId);
            }
            else
            {
                var normalizedEventInstanceHandle = NormalizeHandle(eventInstanceHandle);

                eventInstanceResult = await eventInstancesQuery.FirstOrDefaultAsync(eventInstance => eventInstance.UserId == user.Id && eventInstance.EventSeriesId == eventSeriesId && eventInstance.Handle == normalizedEventInstanceHandle);
            }

            if (eventInstanceResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(eventInstanceResult.Visibility))
            {
                return await eventInstanceFunction(eventInstanceResult);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserEventAsync(string userHandle, string eventSeriesHandle, Func<EventSeries, Task<IActionResult>> eventSeriesFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            EventSeries? eventSeriesResult;

            var eventSeriesQuery = context.EventSeries.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                eventSeriesQuery = eventSeriesQuery.AsNoTracking();
            }

            if (Guid.TryParse(eventSeriesHandle, out Guid eventId))
            {
                eventSeriesResult = await eventSeriesQuery.FirstOrDefaultAsync(eventSeries => eventSeries.UserId == user.Id && eventSeries.Id == eventId);
            }
            else
            {
                var normalizedEventSeriesHandle = NormalizeHandle(eventSeriesHandle);

                eventSeriesResult = await eventSeriesQuery.FirstOrDefaultAsync(eventSeries => eventSeries.UserId == user.Id && eventSeries.Handle == normalizedEventSeriesHandle);
            }

            if (eventSeriesResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(eventSeriesResult.Visibility))
            {
                return await eventSeriesFunction(eventSeriesResult);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserWorkAsync(string userHandle, string workHandle, WorkType workType, Func<Work, Task<IActionResult>> workFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            Work? workResult;

            var worksQuery = context.Works.Where(work => work.Type == workType);

            if (settings?.AsNoTracking ?? true)
            {
                worksQuery = worksQuery.AsNoTracking();
            }

            if (Guid.TryParse(workHandle, out Guid workId))
            {
                workResult = await worksQuery.FirstOrDefaultAsync(work => work.UserId == user.Id && work.Id == workId);
            }
            else
            {
                var normalizedWorkHandle = NormalizeHandle(workHandle);

                workResult = await worksQuery.FirstOrDefaultAsync(work => work.UserId == user.Id && work.Handle == normalizedWorkHandle);
            }

            if (workResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(workResult.Visibility))
            {
                return await workFunction(workResult);
            }

            return CannotReadResponse();
        }, settings);

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

        [NonAction]
        private async Task<bool> CanReadAsync(Visibility visibility)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, visibility, "CanRead");
            return authorizationResult.Succeeded;
        }

        [NonAction]
        private async Task<bool> CanWriteSpaceAsync(Space space)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, space, "CanWriteSpace");
            return authorizationResult.Succeeded;
        }
    }

    public class ContextSettings
    {
        public bool AccessRequired { get; set; } = false;
        public bool AsNoTracking { get; set; } = true;
        public bool IncludeImages { get; set; } = false;
    }
}
