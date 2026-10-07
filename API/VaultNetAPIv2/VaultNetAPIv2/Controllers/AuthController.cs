using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
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
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            var result = await _authService.RegisterAsync(registerDto);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var result = await _authService.LoginAsync(loginDto);

            if (result.IsSuccess)
            {

                var accessToken = result.Data.GetType().GetProperty("AccessToken")?.GetValue(result.Data, null)?.ToString();
                var refreshToken = result.Data.GetType().GetProperty("RefreshToken")?.GetValue(result.Data, null)?.ToString();

                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(5)
                };

                Response.Cookies.Append("accessToken", accessToken!, accessCookieOptions);

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                };

                Response.Cookies.Append("refreshToken", refreshToken!, refreshCookieOptions);

                result.Data = null;

            }

            return StatusCode(result.StatusCode,result);

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.LogoutAsync(refreshToken);

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("logout-all")]
        [Authorize]
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> LogoutAllDevices()
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _authService.LogoutAllDevicesAsync(userId);

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (result.IsSuccess && result.Data != null)
            {

                var newAccessToken = result.Data.GetType().GetProperty("AccessToken")?.GetValue(result.Data, null)?.ToString();
                var newRefreshToken = result.Data.GetType().GetProperty("RefreshToken")?.GetValue(result.Data, null)?.ToString();

                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(5)
                };

                Response.Cookies.Append("accessToken", newAccessToken!, accessCookieOptions);

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                };

                Response.Cookies.Append("refreshToken", newRefreshToken!, refreshCookieOptions);

                result.Data = null;

            }

            else
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }
    }
}
