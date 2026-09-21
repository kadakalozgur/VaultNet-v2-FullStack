using VaultNetAPIv2.Data;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Models;
using VaultNetAPIv2.Results;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly AppDbContext _context;
        IEncryptionService _encryptionService;

        public CategoryService(AppDbContext context, IEncryptionService encryptionService)
        {     
            _context = context;    
            _encryptionService = encryptionService;
        }

        public async Task<ServiceResult> CreateCategoryAsync(Guid userId, CreateCategoryDto createCategoryDto)
        {

            var newCategory = new Category
            {
                UserId = userId,
                EncryptedName = _encryptionService.Encrypt(createCategoryDto.CategoryName.Trim())

            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Category created successfully."
            };

        }

    }
}
