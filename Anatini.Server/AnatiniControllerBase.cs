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
using Npgsql;
using UnreachableException = System.Diagnostics.UnreachableException;

namespace Anatini.Server
{
    public class AnatiniControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : ControllerBase
    {
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
        private IActionResult CannotReadResponse() => IsAuthenticated ? Forbid() : Unauthorized();

        public ApplicationDbContext Context => context;
        public UserManager<ApplicationUser> UserManager => userManager;
        public IBlobService BlobService => blobService;
        private IAuthorizationService AuthorizationService => HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        public Guid? UserIdOrNull => TryGetUserId(out Guid userId) ? userId : null;

        [NonAction]
        public bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        [NonAction]
        public string NormalizeHandle(string handle) => handle.ToLower();
        [NonAction]
        public string? NormalizeHandleOrNull(string? handle) => handle != null ? NormalizeHandle(handle) : null;
        [NonAction]
        public string NormalizeName(string name) => userManager.NormalizeName(name);
        [NonAction]
        public string NormalizeEmail(string email) => userManager.NormalizeEmail(email);

        [NonAction]
        public async Task<IActionResult> UsingAccountAsync(Func<ApplicationUser, Task<IActionResult>> accountFunction, ContextSettings? settings = null)
        {
            var usersQueryable = context.Users.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                usersQueryable = usersQueryable.AsNoTracking();
            }

            usersQueryable = usersQueryable.Include(user => user.Images);
            usersQueryable = usersQueryable.Include(user => user.SpaceRelationships.Where(userSpaceRelationship => userSpaceRelationship.Label == UserSpaceRelationshipLabel.IsOwned)).ThenInclude(userSpaceRelationship => userSpaceRelationship.TargetSpace);

            if (TryGetUserId(out Guid userId))
            {
                var user = await usersQueryable.FirstOrDefaultAsync(user => user.Id == userId);

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

            var usersQueryable = context.Users.AsQueryable();

            if (settings != null)
            {
                if (settings.AsNoTracking)
                {
                    usersQueryable = usersQueryable.AsNoTracking();
                }

                if (settings.IncludeImages)
                {
                    usersQueryable = usersQueryable.Include((user) => user.Images);
                }
            }

            if (TryGetUserId(out Guid sourceUserId))
            {
                usersQueryable = usersQueryable.Include(user => user.ReceivedUserRelationships.Where(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId));
            }

            if (Guid.TryParse(userHandle, out Guid userId))
            {
                userResult = await usersQueryable.FirstOrDefaultAsync(user => user.Id == userId);
            }
            else
            {
                var normalizedUserHandle = NormalizeHandle(userHandle);

                userResult = await usersQueryable.FirstOrDefaultAsync(user => user.Handle == normalizedUserHandle || user.Handles.Any(handle => handle.Handle == normalizedUserHandle));
            }

            if (userResult == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (await CanWriteUserAsync(userResult))
                {
                    return await userFunction(userResult);
                }

                return CannotReadResponse();
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

            var spacesQueryable = context.Spaces.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                spacesQueryable = spacesQueryable.AsNoTracking();
            }

            spacesQueryable = spacesQueryable.Include(space => space.Images);
            spacesQueryable = spacesQueryable.Include(space => space.Settings);

            if (Guid.TryParse(spaceHandle, out Guid spaceId))
            {
                spaceResult = await spacesQueryable.FirstOrDefaultAsync(space => space.Id == spaceId);
            }
            else
            {
                var normalizedSpaceHandle = NormalizeHandle(spaceHandle);

                spaceResult = await spacesQueryable.FirstOrDefaultAsync(space => space.Handle == normalizedSpaceHandle || space.Handles.Any(handle => handle.Handle == normalizedSpaceHandle));
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
        public async Task<IActionResult> UsingContentAsync<T>(Guid contentId, Func<T, Task<IActionResult>> contentFunction, ContextSettings? settings = null) where T : Content
        {
            T? contentResult;

            var contentsQueryable = context.Contents.OfType<T>();

            if (settings?.AsNoTracking ?? true)
            {
                contentsQueryable = contentsQueryable.AsNoTracking();
            }

            contentsQueryable = contentsQueryable.Include(content => content.User);
            contentsQueryable = contentsQueryable.Include(content => content.Space);

            contentResult = await contentsQueryable.FirstOrDefaultAsync(content => content.Id == contentId);

            if (contentResult == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (contentResult.User != null && await CanWriteUserAsync(contentResult.User))
                {
                    return await contentFunction(contentResult);
                }
                else if (contentResult.Space != null && await CanWriteSpaceAsync(contentResult.Space))
                {
                    return await contentFunction(contentResult);
                }

                return CannotReadResponse();
            }

            if (await CanReadAsync(contentResult.Visibility))
            {
                return await contentFunction(contentResult);
            }

            return CannotReadResponse();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpaceContentAsync<T>(string spaceHandle, string contentHandle, Func<T, Space, Task<IActionResult>> contentFunction, ContextSettings? settings = null) where T : Content => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            T? contentResult;

            var contentsQueryable = context.Contents.OfType<T>();

            if (settings?.AsNoTracking ?? true)
            {
                contentsQueryable = contentsQueryable.AsNoTracking();
            }

            if (Guid.TryParse(contentHandle, out Guid contentId))
            {
                contentResult = await contentsQueryable.FirstOrDefaultAsync(content => content.SpaceId == space.Id && content.Id == contentId);
            }
            else
            {
                var normalizedContentHandle = NormalizeHandle(contentHandle);

                contentResult = await contentsQueryable.FirstOrDefaultAsync(content => content.SpaceId == space.Id && content.Handle == normalizedContentHandle);
            }

            if (contentResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(contentResult.Visibility))
            {
                return await contentFunction(contentResult, space);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserContentAsync<T>(string userHandle, string contentHandle, Func<T, ApplicationUser, Task<IActionResult>> contentFunction, ContextSettings? settings = null) where T : Content => await UsingUserAsync(userHandle, async (user) =>
        {
            T? contentResult;

            var contentsQueryable = context.Contents.OfType<T>();

            if (settings?.AsNoTracking ?? true)
            {
                contentsQueryable = contentsQueryable.AsNoTracking();
            }

            if (TryGetUserId(out Guid sourceUserId))
            {
                contentsQueryable = contentsQueryable.Include(work => work.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));
            }

            if (Guid.TryParse(contentHandle, out Guid contentId))
            {
                contentResult = await contentsQueryable.FirstOrDefaultAsync(content => content.UserId == user.Id && content.Id == contentId);
            }
            else
            {
                var normalizedContentHandle = NormalizeHandle(contentHandle);

                contentResult = await contentsQueryable.FirstOrDefaultAsync(content => content.UserId == user.Id && content.Handle == normalizedContentHandle);
            }

            if (contentResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(contentResult.Visibility))
            {
                return await contentFunction(contentResult, user);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserEventOccurrenceAsync(string userHandle, string eventHandle, string eventOccurrenceHandle, Func<EventOccurrence, ApplicationUser, Task<IActionResult>> eventOccurrenceFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            EventOccurrence? eventOccurrenceResult;

            var eventOccurrencesQueryable = context.EventOccurrences.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.AsNoTracking();
            }

            eventOccurrencesQueryable.Include(eventOccurrence => eventOccurrence.Parent);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.Include(eventOccurrence => eventOccurrence.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));
            }

            if (!Guid.TryParse(eventHandle, out Guid eventId))
            {
                var normalizedEventHandle = NormalizeHandle(eventHandle);

                var @event = await context.Events.FirstOrDefaultAsync(@event => @event.UserId == user.Id && @event.Handle == normalizedEventHandle);

                if (@event == null)
                {
                    return NotFound();
                }

                eventId = @event.Id;
            }

            if (Guid.TryParse(eventOccurrenceHandle, out Guid eventOccurrenceId))
            {
                eventOccurrenceResult = await eventOccurrencesQueryable.FirstOrDefaultAsync(eventOccurrence => eventOccurrence.UserId == user.Id && eventOccurrence.ParentId == eventId && eventOccurrence.Id == eventOccurrenceId);
            }
            else
            {
                var normalizedEventOccurrenceHandle = NormalizeHandle(eventOccurrenceHandle);

                eventOccurrenceResult = await eventOccurrencesQueryable.FirstOrDefaultAsync(eventOccurrence => eventOccurrence.UserId == user.Id && eventOccurrence.ParentId == eventId && eventOccurrence.Handle == normalizedEventOccurrenceHandle);
            }

            if (eventOccurrenceResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(eventOccurrenceResult.Visibility))
            {
                return await eventOccurrenceFunction(eventOccurrenceResult, user);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpaceEventOccurrenceAsync(string spaceHandle, string eventHandle, string eventOccurrenceHandle, Func<EventOccurrence, Task<IActionResult>> eventOccurrenceFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            EventOccurrence? eventOccurrenceResult;

            var eventOccurrencesQueryable = context.EventOccurrences.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.AsNoTracking();
            }

            eventOccurrencesQueryable.Include(eventOccurrence => eventOccurrence.Parent);

            if (TryGetUserId(out Guid sourceUserId))
            {
                eventOccurrencesQueryable = eventOccurrencesQueryable.Include(eventOccurrence => eventOccurrence.UserRelationships.Where(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId));
            }

            if (!Guid.TryParse(eventHandle, out Guid eventId))
            {
                var normalizedEventHandle = NormalizeHandle(eventHandle);

                var @event = await context.Events.FirstOrDefaultAsync(@event => @event.SpaceId == space.Id && @event.Handle == normalizedEventHandle);

                if (@event == null)
                {
                    return NotFound();
                }

                eventId = @event.Id;
            }

            if (Guid.TryParse(eventOccurrenceHandle, out Guid eventOccurrenceId))
            {
                eventOccurrenceResult = await eventOccurrencesQueryable.FirstOrDefaultAsync(eventOccurrence => eventOccurrence.SpaceId == space.Id && eventOccurrence.ParentId == eventId && eventOccurrence.Id == eventOccurrenceId);
            }
            else
            {
                var normalizedEventOccurrenceHandle = NormalizeHandle(eventOccurrenceHandle);

                eventOccurrenceResult = await eventOccurrencesQueryable.FirstOrDefaultAsync(eventOccurrence => eventOccurrence.SpaceId == space.Id && eventOccurrence.ParentId == eventId && eventOccurrence.Handle == normalizedEventOccurrenceHandle);
            }

            if (eventOccurrenceResult == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(eventOccurrenceResult.Visibility))
            {
                return await eventOccurrenceFunction(eventOccurrenceResult);
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
        private async Task<bool> CanWriteUserAsync(ApplicationUser user)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, user, "CanWriteUser");
            return authorizationResult.Succeeded;
        }

        [NonAction]
        private async Task<bool> CanWriteSpaceAsync(Space space)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(User, space, "CanWriteSpace");
            return authorizationResult.Succeeded;
        }

        [NonAction]
        public async Task<IActionResult> AddContentRelationship(ApplicationDbContext context, Guid contentId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userContentRelationship = new ApplicationUserContentRelationship
                {
                    SourceUserId = sourceUserId,
                    TargetContentId = contentId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userContentRelationship);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                }

                return Created();
            }
            else
            {
                return Problem();
            }
        }

        [NonAction]
        public async Task<IActionResult> DeleteContentRelationship(ApplicationDbContext context, Guid contentId, UserContentRelationshipLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userContentRelationship = await context.UserContentRelationships.FirstOrDefaultAsync(userContentRelationship => userContentRelationship.TargetContentId == contentId && userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == label);

                if (userContentRelationship != null)
                {
                    context.Remove(userContentRelationship);
                    await context.SaveChangesAsync();
                }

                return NoContent();
            }
            else
            {
                return Problem();
            }
        }

        [NonAction]
        public void ApplyQueryFilters(IQueryable<Content> contentsQueryable, ContentsQuery contentsQuery, Guid sourceUserId)
        {
            if (contentsQuery.Bookmarked == "only")
            {
                contentsQueryable = contentsQueryable.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
            }
            else if (contentsQuery.Bookmarked == "hide")
            {
                contentsQueryable = contentsQueryable.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsBookmarked));
            }

            if (contentsQuery.Starred == "only")
            {
                contentsQueryable = contentsQueryable.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
            }
            else if (contentsQuery.Starred == "hide")
            {
                contentsQueryable = contentsQueryable.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsStarred));
            }

            if (contentsQuery.Dismissed == "only")
            {
                contentsQueryable = contentsQueryable.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
            }
            else if (contentsQuery.Dismissed != "show")
            {
                contentsQueryable = contentsQueryable.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsDismissed));
            }

            if (contentsQuery.Collected == "only")
            {
                contentsQueryable = contentsQueryable.Where(work => work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
            }
            else if (contentsQuery.Collected == "hide")
            {
                contentsQueryable = contentsQueryable.Where(work => !work.UserRelationships.Any(userContentRelationship => userContentRelationship.SourceUserId == sourceUserId && userContentRelationship.Label == UserContentRelationshipLabel.IsCollected));
            }

            if (contentsQuery.Followed == "only")
            {
                contentsQueryable = contentsQueryable.Where(work => work.User != null && work.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
            }
            else if (contentsQuery.Followed == "hide")
            {
                contentsQueryable = contentsQueryable.Where(work => work.User != null && !work.User.ReceivedUserRelationships.Any(userUserRelationship => userUserRelationship.SourceUserId == sourceUserId && userUserRelationship.Label == UserUserRelationshipLabel.IsFollowed));
            }
        }
    }

    public class ContextSettings
    {
        public bool AccessRequired { get; set; } = false;
        public bool AsNoTracking { get; set; } = true;
        public bool IncludeImages { get; set; } = false;
    }
}
