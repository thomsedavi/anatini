using System.Diagnostics;
using System.Security.Claims;
using Anatini.Server.Common;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server
{
    public class AnatiniControllerBase(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        private bool TryGetUserId(out Guid userId) => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        public string NormalizeHandle(string handle) => handle.ToLower();
        public string NormalizeName(string name) => userManager.NormalizeName(name);
        public string NormalizeEmail(string email) => userManager.NormalizeEmail(email);
        public UserManager<ApplicationUser> UserManager => userManager;

        [NonAction]
        public async Task<IActionResult> UsingContextAsync(Func<ApplicationDbContext, Task<IActionResult>> contextFunction)
        {
            try
            {
                return await contextFunction(context);
            }
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict();
            }
        }

        [NonAction]
        public async Task<IActionResult> UsingAccountContextAsync(Func<ApplicationUser, ApplicationDbContext, Task<IActionResult>> userContextFunction, ContextSettings? settings = null) => await UsingAccountAsync(async (user) =>
        {
            return await userContextFunction(user, context);
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
                .Include(user => user.ChannelEdges.Where(userChannelEdge => userChannelEdge.Label == UserChannelEdgeLabel.Owner)).ThenInclude(userChannelEdge => userChannelEdge.Channel);

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
        public async Task<IActionResult> UsingChannelContextAsync(string channelHandle, Func<Channel, ApplicationDbContext, Task<IActionResult>> channelContextFunction, ContextSettings? settings = null) => await UsingChannelAsync(channelHandle, async (channel) =>
        {
            return await channelContextFunction(channel, context);
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingChannelNoteContextAsync(string channelHandle, string noteHandle, Func<Note, ApplicationDbContext, Task<IActionResult>> noteContextFunction, ContextSettings? settings = null) => await UsingChannelNoteAsync(channelHandle, noteHandle, async (note) =>
        {
            return await noteContextFunction(note, context);
        }, settings);


        [NonAction]
        public async Task<IActionResult> UsingChannelAsync(string channelHandle, Func<Channel, Task<IActionResult>> channelFunction, ContextSettings? settings = null)
        {
            Channel? channel;

            var channels = context.Channels.AsQueryable();

            if (settings?.AsNoTracking ?? true)
            {
                channels = channels.AsNoTracking();
            }

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
                if (TryGetUserId(out Guid userId) && await context.UserChannelEdges.AnyAsync(userChannel => userChannel.UserId == userId && userChannel.ChannelId == channel.Id && userChannel.Label == UserChannelEdgeLabel.Owner))
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
        public async Task<IActionResult> UsingChannelPostContextAsync(string channelHandle, string postHandle, Func<Post, ApplicationDbContext, Task<IActionResult>> postContextFunction, ContextSettings? settings = null) => await UsingChannelPostAsync(channelHandle, postHandle, async (post) =>
        {
            return await postContextFunction(post, context);
        }, settings);

        [NonAction]
        public async Task<IActionResult> UsingChannelPostAsync(string channelHandle, string postHandle, Func<Post, Task<IActionResult>> postFunction, ContextSettings? settings = null)
        {
            return await UsingChannelAsync(channelHandle, async (channel) =>
            {
                ChannelPost? channelPost;

                var channelPosts = context.ChannelPosts.AsQueryable();

                if (settings?.AsNoTracking ?? true)
                {
                    channelPosts = channelPosts.AsNoTracking();
                }

                if (Guid.TryParse(postHandle, out Guid postId))
                {
                    channelPost = await channelPosts.Include(channelPost => channelPost.Post).FirstOrDefaultAsync(channelPost => channelPost.ChannelId == channel.Id && channelPost.PostId == postId);
                }
                else
                {
                    var normalizedPostHandle = NormalizeHandle(postHandle);

                    channelPost = await channelPosts.Include(channelPost => channelPost.Post).FirstOrDefaultAsync(channelPost => channelPost.ChannelId == channel.Id && channelPost.Handle == normalizedPostHandle);
                }

                if (channelPost == null)
                {
                    return NotFound();
                }

                var post = channelPost.Post;

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
        }

        [NonAction]
        public async Task<IActionResult> UsingChannelNoteAsync(string channelHandle, string noteHandle, Func<Note, Task<IActionResult>> noteFunction, ContextSettings? settings = null)
        {
            return await UsingChannelAsync(channelHandle, async (channel) =>
            {
                ChannelNote? channelNote;

                var channelNotes = context.ChannelNotes.AsQueryable();

                if (settings?.AsNoTracking ?? true)
                {
                    channelNotes = channelNotes.AsNoTracking();
                }

                if (Guid.TryParse(noteHandle, out Guid noteId))
                {
                    channelNote = await channelNotes.Include(channelNote => channelNote.Note).FirstOrDefaultAsync(channelNote => channelNote.ChannelId == channel.Id && channelNote.NoteId == noteId);
                }
                else
                {
                    var normalizedNoteHandle = NormalizeHandle(noteHandle);

                    channelNote = await channelNotes.Include(channelNote => channelNote.Note).FirstOrDefaultAsync(channelNote => channelNote.ChannelId == channel.Id && channelNote.Handle == normalizedNoteHandle);
                }

                if (channelNote == null)
                {
                    return NotFound();
                }

                var note = channelNote.Note;

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
        }

        [NonAction]
        public async Task<IActionResult> UsingAccountNoteAsync(string noteHandle, Func<Note, Task<IActionResult>> noteFunction, ContextSettings? settings = null)
        {
            return await UsingAccountAsync(async (user) =>
            {
                ApplicationUserNote? userNote;

                var userNotes = context.UserNotes.AsQueryable();

                if (settings?.AsNoTracking ?? true)
                {
                    userNotes = userNotes.AsNoTracking();
                }

                if (Guid.TryParse(noteHandle, out Guid noteId))
                {
                    userNote = await userNotes.Include(userNote => userNote.Note).FirstOrDefaultAsync(userNote => userNote.UserId == user.Id && userNote.NoteId == noteId);
                }
                else
                {
                    var normalizedNoteHandle = NormalizeHandle(noteHandle);

                    userNote = await userNotes.Include(userNote => userNote.Note).FirstOrDefaultAsync(userNote => userNote.UserId == user.Id && userNote.Handle == normalizedNoteHandle);
                }

                if (userNote == null)
                {
                    return NotFound();
                }

                var note = userNote.Note;

                return await noteFunction(note);
            }, settings);
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

    public class ContextSettings
    {
        public bool AccessRequired { get; set; } = false;
        public bool AsNoTracking { get; set; } = true;
        public bool IncludeImages { get; set; } = false;
    }
}
