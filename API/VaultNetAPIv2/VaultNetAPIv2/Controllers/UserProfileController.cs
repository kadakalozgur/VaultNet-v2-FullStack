using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {

        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.GetUserProfileAsync(userId);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("update-profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile photo)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.UpdateProfilePictureAsync(userId, photo);

            return StatusCode(result.StatusCode, result);

        }
    }
}
