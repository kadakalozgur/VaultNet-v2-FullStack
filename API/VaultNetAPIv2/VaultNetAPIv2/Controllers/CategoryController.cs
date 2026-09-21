using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Services;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.CreateCategoryAsync(userId, createCategoryDto);

            return StatusCode(result.StatusCode, result);

        }
    }
}
