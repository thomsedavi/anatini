using Anatini.Server.Contents.Extensions;
using Anatini.Server.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Contents
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentsController : AnatiniControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetContents([FromQuery] Query query) => await UsingContextAsync(async (context) =>
        {
            if (query.Week != null)
            {
                var value = $"{AttributeContentType.Week}:{query.Week}";

                var attributeContentsPage = await context.Context.AttributeContents.WithPartitionKey(value).OrderBy(a => a.ItemId).ToPageAsync(10, null);

                return Ok(new { AttributeContents = attributeContentsPage.Values.Select(attributeContent => attributeContent.ToAttributeContentDto()), attributeContentsPage.ContinuationToken });
            }

            return BadRequest();
        });
    }

    public class Query
    {
        public string? Week { get; set; }
    }
}
