using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface ICategoryService
    {

        Task<ServiceResult> CreateCategoryAsync(Guid userId, CreateCategoryDto createCategoryDto);

        Task<ServiceResult> GetCategoriesAsync(Guid userId, int pageNumber, string? searchQuery = null);

    }
}
