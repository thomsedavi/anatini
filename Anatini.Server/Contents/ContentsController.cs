using Anatini.Server.Context;
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
        public async Task<IActionResult> GetContents([FromQuery] Query query)
        {
            async Task<IActionResult> contextFunctionAsync(AnatiniContext context)
            {
                if (query.Week != null)
                {
                    var value = $"{AttributeContentType.Week}:{query.Week}";

                    var attributeContents = await context.Context.AttributeContents.Where(a => a.Value == value).OrderByDescending(a => a.Timestamp).Skip(1).Take(10).ToListAsync();

                    return Ok(new { Contents = attributeContents });
                }

                return BadRequest();
            }

            return await UsingContextAsync(contextFunctionAsync);
        }
    }

    public class Query
    {
        public string? Week { get; set; }
    }
}
