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
        public async Task<IActionResult> GetVaultItems([FromQuery] int pageNumber = 1)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _vaultItemService.GetItemsAsync(userId,pageNumber);

            return StatusCode(result.StatusCode, result);

        }

    }
}
