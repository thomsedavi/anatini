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
        public async Task<IActionResult> UsingAccountNoteContextAsync(string noteHandle, Func<Note, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingAccountNoteAsync(noteHandle, async (note) =>
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
        public async Task<IActionResult> UsingChannelContextAsync(string channelHandle, Func<Channel, ApplicationDbContext, Task<IActionResult>> channelContextFunction, ContextSettings? settings = null) => await UsingChannelAsync(channelHandle, async (channel) =>
        {
            try
            {
                return await channelContextFunction(channel, context);
            }
            catch (Exception ex)
            {
                return ExceptionResult(ex);
            }
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingChannelNoteContextAsync(string channelHandle, string noteHandle, Func<Note, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingChannelNoteAsync(channelHandle, noteHandle, async (note) =>
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
        public async Task<IActionResult> UsingUserNoteContextAsync(string userHandle, string noteHandle, Func<Note, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingUserNoteAsync(userHandle, noteHandle, async (note) =>
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
        public async Task<IActionResult> UsingChannelPostContextAsync(string channelHandle, string postHandle, Func<Post, ApplicationDbContext, Task<IActionResult>> postContextFunction, ContextSettings? settings = null) => await UsingChannelPostAsync(channelHandle, postHandle, async (post) =>
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
                .Include(user => user.ChannelEdges.Where(userChannelEdge => userChannelEdge.Label == UserChannelEdgeLabel.Owner)).ThenInclude(userChannelEdge => userChannelEdge.TargetChannel);

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
        public async Task<IActionResult> UsingChannelAsync(string channelHandle, Func<Channel, Task<IActionResult>> channelFunction, ContextSettings? settings = null)
        {
            Channel? channel;

            var channels = context.Channels.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                channels = channels.AsNoTracking();
            }

            channels = channels.Include(channel => channel.Images);

            if (Guid.TryParse(channelHandle, out Guid channelId))
            {
                channel = await channels.FirstOrDefaultAsync(channel => channel.Id == channelId);
            }
            else
            {
                var normalizedChannelHandle = NormalizeHandle(channelHandle);

                channel = await channels.FirstOrDefaultAsync(channel => channel.Handle == normalizedChannelHandle || channel.Handles.Any(handle => handle.Handle == normalizedChannelHandle));
            }

            if (channel == null)
            {
                return NotFound();
            }

            if (settings?.AccessRequired ?? false)
            {
                if (TryGetUserId(out Guid userId) && await context.UserChannelEdges.AnyAsync(userChannel => userChannel.SourceUserId == userId && userChannel.TargetChannelId == channel.Id && userChannel.Label == UserChannelEdgeLabel.Owner))
                {
                    return await channelFunction(channel);
                }

                return Unauthorized();
            }

            if (channel.Visibility == Visibility.Public)
            {
                return await channelFunction(channel);
            }

            if (!IsAuthenticated)
            {
                return NotFound();
            }

            if (channel.Visibility == Visibility.Protected)
            {
                return await channelFunction(channel);
            }

            // TODO handle Private
            return NotFound();
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelPostAsync(string channelHandle, string postHandle, Func<Post, Task<IActionResult>> postFunction, ContextSettings? settings = null) => await UsingChannelAsync(channelHandle, async (channel) =>
        {
            Post? post;

            var posts = context.Posts.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                posts = posts.AsNoTracking();
            }

            if (Guid.TryParse(postHandle, out Guid postId))
            {
                post = await posts.FirstOrDefaultAsync(post => post.ChannelId == channel.Id && post.Id == postId);
            }
            else
            {
                var normalizedPostHandle = NormalizeHandle(postHandle);

                post = await posts.FirstOrDefaultAsync(post => post.ChannelId == channel.Id && post.Handle == normalizedPostHandle);
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
        public async Task<IActionResult> UsingChannelNoteAsync(string channelHandle, string noteHandle, Func<Note, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingChannelAsync(channelHandle, async (channel) =>
        {
            Note? note;

            var notes = context.Notes.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                notes = notes.AsNoTracking();
            }

            if (Guid.TryParse(noteHandle, out Guid noteId))
            {
                note = await notes.FirstOrDefaultAsync(note => note.ChannelId == channel.Id && note.Id == noteId);
            }
            else
            {
                var normalizedNoteHandle = NormalizeHandle(noteHandle);

                note = await notes.FirstOrDefaultAsync(note => note.ChannelId == channel.Id && note.Handle == normalizedNoteHandle);
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
        public async Task<IActionResult> UsingUserNoteAsync(string userHandle, string noteHandle, Func<Note, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingUserAsync(userHandle, async (user) =>
        {
            Note? note;

            var notes = context.Notes.AsQueryable();

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
        public async Task<IActionResult> UsingAccountNoteAsync(string noteHandle, Func<Note, Task<IActionResult>> noteFunction, ContextSettings? settings = null) => await UsingAccountAsync(async (user) =>
        {
            Note? note;

            var notes = context.Notes.AsQueryable();

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
