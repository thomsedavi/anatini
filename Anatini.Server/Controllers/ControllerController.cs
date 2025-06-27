using anatini.Server.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerController : ControllerBase
    {
        [HttpGet("action/{id:int}")]
        [Authorize]
        [RequiresIdClaim]
        public IActionResult Get(int id)
        {
            return Ok(id);
        }
    }
}
