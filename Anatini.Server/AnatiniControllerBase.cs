using System.Diagnostics;
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

namespace Anatini.Server
{
    public class AnatiniControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : ControllerBase
    {
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        public bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        public string NormalizeHandle(string handle) => handle.ToLower();
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
            ApplicationUser? user;

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
                user = await usersQuery.FirstOrDefaultAsync(user => user.Id == userId);
            }
            else
            {
                var normalizedUserHandle = NormalizeHandle(userHandle);

                user = await usersQuery.FirstOrDefaultAsync(user => user.Handle == normalizedUserHandle || user.Handles.Any(handle => handle.Handle == normalizedUserHandle));
            }

            if (user == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(user.Visibility))
            {
                return await userFunction(user);
            }

            return CannotReadResponse();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpaceAsync(string spaceHandle, Func<Space, Task<IActionResult>> spaceFunction, ContextSettings? settings = null)
        {
            Space? space;

            var spacesQuery = context.Spaces.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                spacesQuery = spacesQuery.AsNoTracking();
            }

            spacesQuery = spacesQuery.Include(space => space.Images);

            if (Guid.TryParse(spaceHandle, out Guid spaceId))
            {
                space = await spacesQuery.FirstOrDefaultAsync(space => space.Id == spaceId);
            }
            else
            {
                var normalizedSpaceHandle = NormalizeHandle(spaceHandle);

                space = await spacesQuery.FirstOrDefaultAsync(space => space.Handle == normalizedSpaceHandle || space.Handles.Any(handle => handle.Handle == normalizedSpaceHandle));
            }

            if (space == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (await CanWriteSpaceAsync(space))
                {
                    return await spaceFunction(space);
                }

                return CannotReadResponse();
            }

            if (await CanReadAsync(space.Visibility))
            {
                return await spaceFunction(space);
            }

            return CannotReadResponse();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpacePostAsync(string spaceHandle, string postHandle, Func<Content, Task<IActionResult>> postFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            Content? post;

            var postsQuery = context.Posts;

            if (settings?.AsNoTracking ?? true)
            {
                postsQuery = postsQuery.AsNoTracking();
            }

            if (Guid.TryParse(postHandle, out Guid postId))
            {
                post = await postsQuery.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Id == postId);
            }
            else
            {
                var normalizedPostHandle = NormalizeHandle(postHandle);

                post = await postsQuery.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Handle == normalizedPostHandle);
            }

            if (post == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(post.Visibility))
            {
                return await postFunction(post);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpaceNoteAsync(string spaceHandle, string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            Content? note;

            var notesQuery = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notesQuery = notesQuery.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notesQuery.FirstOrDefaultAsync(note => note.SpaceId == space.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notesQuery.FirstOrDefaultAsync(note => note.SpaceId == space.Id && note.Handle == normalizedNoteHandle);
            }

            if (note == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(note.Visibility))
            {
                return await noteFunction(note);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserEventAsync(string userHandle, string eventSeriesHandle, Func<EventSeries, Task<IActionResult>> eventFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            EventSeries? eventSeries;

            var eventSeriesQuery = context.EventSeries.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                eventSeriesQuery = eventSeriesQuery.AsNoTracking();
            }

            if (Guid.TryParse(eventSeriesHandle, out Guid eventId))
            {
                eventSeries = await eventSeriesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Id == eventId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(eventSeriesHandle);

                eventSeries = await eventSeriesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Handle == normalizedNoteHandle);
            }

            if (eventSeries == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(eventSeries.Visibility))
            {
                return await eventFunction(eventSeries);
            }

            return CannotReadResponse();
        }, settings);


        [NonAction]
        public async Task<IActionResult> UsingUserNoteAsync(string userHandle, string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            Content? note;

            var notesQuery = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notesQuery = notesQuery.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Handle == normalizedNoteHandle);
            }

            if (note == null)
            {
                return NotFound();
            }

            if (await CanReadAsync(note.Visibility))
            {
                return await noteFunction(note);
            }

            return CannotReadResponse();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingAccountNoteAsync(string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingAccountAsync(async (user) =>
        {
            Content? note;

            var notesQuery = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notesQuery = notesQuery.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notesQuery.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Handle == normalizedNoteHandle);
            }

            if (note == null)
            {
                return NotFound();
            }

            return await noteFunction(note);
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
