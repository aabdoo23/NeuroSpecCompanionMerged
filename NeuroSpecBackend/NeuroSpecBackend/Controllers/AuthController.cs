using Microsoft.AspNetCore.Mvc;
using NeuroSpecBackend.Services;
using NeuroSpec.Shared.Models.DTO;
using System.Threading.Tasks;

namespace NeuroSpecBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] Patient newPatient)
        {
            var result = await _authService.SignUp(newPatient);
            if (result != null)
            {
                return BadRequest(result);
            }
            return Ok("Sign-up successful");
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var token = await _authService.SignIn(request.Username, request.Password);
            if (token == null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(new { Token = token });
        }
    }

    public class SignInRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
