using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using VaultNetAPIv2.DTOs;
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
        [EnableRateLimiting("ReadLimit")]
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

        [HttpPatch("update-profile-picture")]
        [EnableRateLimiting("UserLimit")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureDto updateProfilePictureDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.UpdateProfilePictureAsync(userId, updateProfilePictureDto);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPatch("update-email-address")]
        [EnableRateLimiting("UserLimit")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto updateEmailDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.UpdateEmailAsync(userId, updateEmailDto);

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }

        [HttpPatch("change-password")]
        [EnableRateLimiting("UserLimit")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.ChangePasswordAsync(userId,changePasswordDto);

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }

        [HttpDelete("delete-account")]
        [EnableRateLimiting("UserLimit")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto deleteAccountDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.DeleteAccountAsync(userId,deleteAccountDto);

            if (result.IsSuccess)
            {
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");
            }

            return StatusCode(result.StatusCode, result);

        }

        [HttpDelete("remove-profile-picture")]
        [EnableRateLimiting("UserLimit")]
        public async Task<IActionResult> RemoveProfilePicture()
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _userProfileService.RemoveProfilePictureAsync(userId);

            return StatusCode(result.StatusCode, result);

        }
    }
}
