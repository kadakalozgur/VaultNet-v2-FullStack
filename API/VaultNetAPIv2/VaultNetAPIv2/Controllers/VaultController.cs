using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultController : ControllerBase
    {

        private readonly IVaultItemService _vaultItemService;

        public VaultController(IVaultItemService vaultItemService)
        {
            _vaultItemService = vaultItemService;
        }

        [HttpPost("add-vault-item")]
        public async Task<IActionResult> AddItem([FromBody] AddVaultItemDto addVaultItemDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.AddItemAsync(userId, addVaultItemDto);

            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("get-vault-items")]
        public async Task<IActionResult> GetVaultItems([FromQuery] int pageNumber = 1, [FromQuery] string? searchQuery = null)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.GetItemsAsync(userId,pageNumber, searchQuery);

            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.GetItemByIdAsync(userId,id);

            return StatusCode(result.StatusCode, result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.DeleteItemAsync(userId,id);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateVaultItemDto updateVaultItemDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.UpdateItemAsync(userId, id, updateVaultItemDto);

            return StatusCode(result.StatusCode, result);

        }

    }
}
