using System.Net.Mime;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Works.Products
{
    [ApiController]
    [Route("api/users/{userHandle}/products")]
    public class UserProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobService blobService) : AnatiniControllerBase(context, userManager, blobService)
    {
        [HttpPost]
        [Authorize(Policy = "IsTrusted")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostProduct([FromForm] CreateWork createWork) => await UsingAccountAsync(async (user) =>
        {
            string? article = null;

            if (createWork.Article != null)
            {
                var validationResult = HtmlContentService.ValidateAndNormalizeHtml(createWork.Article);

                if (validationResult.ErrorMessage != null)
                {
                    return BadRequest(new { error = validationResult.ErrorMessage });
                }
                else if (validationResult.SanitizedHtml == null)
                {
                    return BadRequest(new { error = "Unknown error" });
                }

                article = validationResult.SanitizedHtml;
            }

            var product = Context.AddUserWorkAsync(WorkType.Product, createWork.Name, createWork.Url, createWork.Visibility, user.Id, (createWork.IsDraft ?? false) ? Status.Draft : Status.Published, DateTime.UtcNow, NormalizeHandleOrNull(createWork.Handle), article);

            await Context.SaveChangesAsync();

            product.User = user;

            return CreatedAtAction(nameof(GetProduct), new { userHandle = user.Handle, productHandle = product.Handle }, await product.ToWorkDtoAsync(NormalizeHandleOrNull(createWork.Handle), BlobService));
        }, new ContextSettings { AccessRequired = true });

        [Authorize]
        [HttpGet("{productHandle}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduct(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return Ok(await product.ToWorkDtoAsync(productHandle, BlobService));
        });

        [Authorize]
        [HttpPost("{productHandle}/bookmark")]
        public async Task<IActionResult> PostProductBookmark(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await AddUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpDelete("{productHandle}/bookmark")]
        public async Task<IActionResult> DeleteProductBookmark(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await DeleteUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasBookmarked);
        });

        [Authorize]
        [HttpPost("{productHandle}/star")]
        public async Task<IActionResult> PostProductStar(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await AddUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpDelete("{productHandle}/star")]
        public async Task<IActionResult> DeleteProductStar(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await DeleteUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasStarred);
        });

        [Authorize]
        [HttpPost("{productHandle}/dismiss")]
        public async Task<IActionResult> PostProductDismiss(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await AddUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasDismissed);
        });

        [Authorize]
        [HttpDelete("{productHandle}/dismiss")]
        public async Task<IActionResult> DeleteProductDismiss(string userHandle, string productHandle) => await UsingUserWorkAsync(userHandle, productHandle, WorkType.Product, async (product) =>
        {
            return await DeleteUserProductEdge(Context, product.Id, UserPostEdgeLabel.HasDismissed);
        });

        private async Task<IActionResult> AddUserProductEdge(ApplicationDbContext context, Guid productId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userProductEdge = new ApplicationUserPostEdge
                {
                    SourceUserId = sourceUserId,
                    TargetPostId = productId,
                    Label = label,
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Add(userProductEdge);

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

        private async Task<IActionResult> DeleteUserProductEdge(ApplicationDbContext context, Guid productId, UserPostEdgeLabel label)
        {
            if (TryGetUserId(out Guid sourceUserId))
            {
                var userProductEdge = await context.UserPostEdges.FirstOrDefaultAsync(userProductEdge => userProductEdge.TargetPostId == productId && userProductEdge.SourceUserId == sourceUserId && userProductEdge.Label == label);

                if (userProductEdge != null)
                {
                    context.Remove(userProductEdge);
                    await context.SaveChangesAsync();
                }

                return NoContent();
            }
            else
            {
                return Problem();
            }
        }
    }
}
