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

            return StatusCode(result.StatusCode, new
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message
            });
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var result = await _authService.LoginAsync(loginDto);

            return StatusCode(result.StatusCode, new
            {
                IsSuccess = result.IsSuccess,
                Data = result.Data,
                Message = result.Message
            });

        }
    }
}
