using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            var result = await _authService.RegisterAsync(registerDto);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var result = await _authService.LoginAsync(loginDto);

            if (result.IsSuccess)
            {

                var tokenValue = result.Data.GetType().GetProperty("Token").GetValue(result.Data, null).ToString();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.Strict, 
                    Expires = DateTime.UtcNow.AddMinutes(15) // You must make this the same as your JWT's expiration date.
                };

                Response.Cookies.Append("jwt_token", tokenValue, cookieOptions);

                result.Data = null; // Tokenı dışarıya yollamamak için.

            }

            return StatusCode(result.StatusCode,result);

        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {

            var result = _authService.Logout();

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("jwt_token");
            }

            return StatusCode(result.StatusCode, result);

        }
    }
}
