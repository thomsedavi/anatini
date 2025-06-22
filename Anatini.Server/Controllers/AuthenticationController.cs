using Microsoft.AspNetCore.Mvc;

namespace anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private static readonly List<SignupForm> users = [];

        [HttpPost("signup")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Signup([FromForm] SignupForm values)
        {
            users.Add(values);

            return Ok(values);
        }
    }

    public class SignupForm
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
