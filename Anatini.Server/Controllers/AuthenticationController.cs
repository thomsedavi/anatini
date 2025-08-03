using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public IActionResult Signup([FromForm] SignupForm request)
        {
            if (request.InviteCode != "1234-5678")
            {
                return BadRequest();
            }

            return Ok();
        }
    }

    public class SignupForm
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Handle { get; set; }
        public required string InviteCode { get; set; }
    }
}
