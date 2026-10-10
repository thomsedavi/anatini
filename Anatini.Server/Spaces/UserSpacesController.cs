using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Spaces
{
    [ApiController]
    [Route("api/users/{userHandle}/spaces")]
    public class UserSpacesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [Authorize]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpaces(string userHandle) => await UsingUserAsync(userHandle, async (user) =>
        {
            var spacesQuery = Context.Spaces.AsQueryable();

            spacesQuery = spacesQuery.AsNoTracking();

            spacesQuery = spacesQuery.Where(space => space.UserRelationships.Any(userSpaceRelationship => userSpaceRelationship.SourceUserId == user.Id && userSpaceRelationship.Label == UserSpaceRelationshipLabel.IsOwned));

            var spaces = await spacesQuery.ToListAsync();

            if (spaces == null)
            {
                return Problem();
            }

            return Ok(await Task.WhenAll(spaces.Select(post => post.ToSpaceDtoAsync(IsAuthenticated, BlobService))));
        });
    }
}
