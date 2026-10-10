using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("WriteLimit")]
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

        [HttpGet("get-category-items")]
        [EnableRateLimiting("ReadLimit")]
        public async Task<IActionResult> GetCategories([FromQuery] int pageNumber = 1, [FromQuery] string? searchQuery = null)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.GetCategoriesAsync(userId, pageNumber, searchQuery);

            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("{id}")]
        [EnableRateLimiting("ReadLimit")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.GetCategoryByIdAsync(userId,id);

            return StatusCode(result.StatusCode, result);

        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("WriteLimit")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.DeleteCategoryAsync(userId, id);

            return StatusCode(result.StatusCode, result);

        }

        [HttpPut("{id}")]
        [EnableRateLimiting("WriteLimit")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.UpdateCategoryAsync(userId, id, updateCategoryDto);

            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("get-user-all-categories")]
        [EnableRateLimiting("ReadLimit")]
        public async Task<IActionResult> GetAllCategories()
        {

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user identity." });
            }

            var result = await _categoryService.GetAllCategoriesAsync(userId);

            return StatusCode(result.StatusCode, result);

        }
    }
}
