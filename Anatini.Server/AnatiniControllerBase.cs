using System.Diagnostics;
using System.Security.Claims;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server
{
    public class AnatiniControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : ControllerBase
    {
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        public bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        public string NormalizeHandle(string handle) => handle.ToLower();
        public string NormalizeName(string name) => userManager.NormalizeName(name);
        public string NormalizeEmail(string email) => userManager.NormalizeEmail(email);
        public UserManager<ApplicationUser> UserManager => userManager;
        public IBlobService BlobService => blobService;

        [NonAction]
        public async Task<IActionResult> UsingContextAsync(Func<ApplicationDbContext, Task<IActionResult>> contextFunction)
        {
            try
            {
                return await contextFunction(context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }

        [NonAction]
        public async Task<IActionResult> UsingAccountContextAsync(Func<ApplicationUser, ApplicationDbContext, Task<IActionResult>> userContextFunction, ContextSettings? settings = null) => await UsingAccountAsync(async (user) =>
        {
            try
            {
                return await userContextFunction(user, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingAccountNoteContextAsync(string noteHandle, Func<Content, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingAccountNoteAsync(noteHandle, async (note) =>
        {
            try
            {
                return await noteContextFunction(note, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpaceContextAsync(string spaceHandle, Func<Space, ApplicationDbContext, Task<IActionResult>> spaceContextFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            try
            {
                return await spaceContextFunction(space, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpaceNoteContextAsync(string spaceHandle, string noteHandle, Func<Content, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingSpaceNoteAsync(spaceHandle, noteHandle, async (note) =>
        {
            try
            {
                return await noteContextFunction(note, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserContextAsync(string userHandle, Func<ApplicationUser, ApplicationDbContext, Task<IActionResult>> userContextFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            try
            {
                return await userContextFunction(user, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserNoteContextAsync(string userHandle, string noteHandle, Func<Content, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
        {
            try
            {
                return await noteContextFunction(note, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpacePostContextAsync(string spaceHandle, string postHandle, Func<Content, ApplicationDbContext, Task<IActionResult>> postContextFunction, ContextSettings? settings = null) => await UsingSpacePostAsync(spaceHandle, postHandle, async (post) =>
        {
            try
            {
                return await postContextFunction(post, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingAccountAsync(Func<ApplicationUser, Task<IActionResult>> accountFunction, ContextSettings? settings = null)
        {
            var users = context.Users.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                users = users.AsNoTracking();
            }

            users = users
                .Include(user => user.Images)
                .Include(user => user.SpaceEdges.Where(userSpaceEdge => userSpaceEdge.Label == UserSpaceEdgeLabel.Owner)).ThenInclude(userSpaceEdge => userSpaceEdge.TargetSpace);

            if (TryGetUserId(out Guid userId))
            {
                var user = await users.FirstOrDefaultAsync(user => user.Id == userId);

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

            var users = context.Users.AsQueryable();

            if (settings != null)
            {
                if (settings.AsNoTracking)
                {
                    users = users.AsNoTracking();
                }

                if (settings.IncludeImages)
                {
                    users = users.Include((user) => user.Images);
                }
            }

            if (TryGetUserId(out Guid sourceUserId))
            {
                users = users.Include(user => user.ReceivedUserEdges.Where(userUserEdge => userUserEdge.SourceUserId == sourceUserId));
            }

            if (Guid.TryParse(userHandle, out Guid userId))
            {
                user = await users.FirstOrDefaultAsync(user => user.Id == userId);
            }
            else
            {
                var normalizedUserHandle = NormalizeHandle(userHandle);

                user = await users.FirstOrDefaultAsync(user => user.Handle == normalizedUserHandle || user.Handles.Any(handle => handle.Handle == normalizedUserHandle));
            }

            if (user == null)
            {
                return NotFound();
            }

            if (user.Visibility == Visibility.Public)
            {
                return await userFunction(user);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (user.Visibility == Visibility.Protected)
            {
                return await userFunction(user);
            }

            // TODO handle Private
            return NotFound();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpaceAsync(string spaceHandle, Func<Space, Task<IActionResult>> spaceFunction, ContextSettings? settings = null)
        {
            Space? space;

            var spaces = context.Spaces.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                spaces = spaces.AsNoTracking();
            }

            spaces = spaces.Include(space => space.Images);

            if (Guid.TryParse(spaceHandle, out Guid spaceId))
            {
                space = await spaces.FirstOrDefaultAsync(space => space.Id == spaceId);
            }
            else
            {
                var normalizedSpaceHandle = NormalizeHandle(spaceHandle);

                space = await spaces.FirstOrDefaultAsync(space => space.Handle == normalizedSpaceHandle || space.Handles.Any(handle => handle.Handle == normalizedSpaceHandle));
            }

            if (space == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (TryGetUserId(out Guid userId) && await context.UserSpaceEdges.AnyAsync(userSpace => userSpace.SourceUserId == userId && userSpace.TargetSpaceId == space.Id && userSpace.Label == UserSpaceEdgeLabel.Owner))
                {
                    return await spaceFunction(space);
                }

                return Unauthorized();
            }

            if (space.Visibility == Visibility.Public)
            {
                return await spaceFunction(space);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (space.Visibility == Visibility.Protected)
            {
                return await spaceFunction(space);
            }

            // TODO handle Private
            return NotFound();
        }

        [NonAction]
        public async Task<IActionResult> UsingSpacePostAsync(string spaceHandle, string postHandle, Func<Content, Task<IActionResult>> postFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            Content? post;

            var posts = context.Posts;

            if (settings?.AsNoTracking ?? true)
            {
                posts = posts.AsNoTracking();
            }

            if (Guid.TryParse(postHandle, out Guid postId))
            {
                post = await posts.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Id == postId);
            }
            else
            {
                var normalizedPostHandle = NormalizeHandle(postHandle);

                post = await posts.FirstOrDefaultAsync(post => post.SpaceId == space.Id && post.Handle == normalizedPostHandle);
            }

            if (post == null)
            {
                return NotFound();
            }

            if (post.Visibility == Visibility.Public)
            {
                return await postFunction(post);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (post.Visibility == Visibility.Protected)
            {
                return await postFunction(post);
            }

            // TODO handle Private
            return NotFound();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingSpaceNoteAsync(string spaceHandle, string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingSpaceAsync(spaceHandle, async (space) =>
        {
            Content? note;

            var notes = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notes = notes.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notes.FirstOrDefaultAsync(note => note.SpaceId == space.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notes.FirstOrDefaultAsync(note => note.SpaceId == space.Id && note.Handle == normalizedNoteHandle);
            }

            if (note == null)
            {
                return NotFound();
            }

            if (note.Visibility == Visibility.Public)
            {
                return await noteFunction(note);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (note.Visibility == Visibility.Protected)
            {
                return await noteFunction(note);
            }

            // TODO handle Private
            return NotFound();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingUserNoteAsync(string userHandle, string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            Content? note;

            var notes = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notes = notes.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notes.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notes.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Handle == normalizedNoteHandle);
            }

            if (note == null)
            {
                return NotFound();
            }

            if (note.Visibility == Visibility.Public)
            {
                return await noteFunction(note);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (note.Visibility == Visibility.Protected)
            {
                return await noteFunction(note);
            }

            // TODO handle Private
            return NotFound();
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingAccountNoteAsync(string noteHandle, Func<Content, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingAccountAsync(async (user) =>
        {
            Content? note;

            var notes = context.Notes;

            if (settings?.AsNoTracking ?? true)
            {
                notes = notes.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notes.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notes.FirstOrDefaultAsync(note => note.UserId == user.Id && note.Handle == normalizedNoteHandle);
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
        private IActionResult ExceptionResult(Exception ex)
        {
            if (ex is DbUpdateException dbUpdateException && dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict();
            }
            else
            {
                return Problem();
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
