using Microsoft.EntityFrameworkCore;
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
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(AppDbContext context, IEncryptionService encryptionService, ILogger<CategoryService> logger)
        {     
            _context = context;    
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<ServiceResult> CreateCategoryAsync(Guid userId, CreateCategoryDto createCategoryDto)
        {

            var currentCategoryCount = await _context.Categories.CountAsync(item => item.UserId == userId);

            if (currentCategoryCount >= 15)
            {

                _logger.LogWarning("Category creation failed. User reached maximum limit (15). UserId: {UserId}", userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "You have reached the maximum limit of 15 category."
                };
            }

            var newCategory = new Category
            {
                UserId = userId,
                EncryptedName = _encryptionService.Encrypt(createCategoryDto.CategoryName.Trim())

            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category created successfully. CategoryId: {CategoryId}, UserId: {UserId}", newCategory.Id, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Category created successfully."
            };

        }

        // Sıkı sıkıya bağımlı yaptım. Benim projemde bu değerde olmasını istiyorum.
        private const int pageSize = 6;
        public async Task<ServiceResult> GetCategoriesAsync(Guid userId, int pageNumber, string? searchQuery = null)
        {

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = _context.Categories
                .AsNoTracking()
                .Where(category => category.UserId == userId);

            // Bu if bloğu search işlemi esnasında çalışıyor.
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {

                var lowerSearchQuery = searchQuery.Trim().ToLower();

                var allEncryptedCategories = await query
                    .Select(item => new
                    {
                        item.Id,
                        item.EncryptedName,
                        item.CreatedAt,
                        item.UpdatedAt,
                    })
                    .ToListAsync();

                var filteredItems = allEncryptedCategories
                    .Select(item => new
                    {
                        item.Id,
                        CategoryName = _encryptionService.Decrypt(item.EncryptedName),
                        item.UpdatedAt,
                        item.CreatedAt
                    })
                    .Where(item => item.CategoryName.ToLower().Contains(lowerSearchQuery))
                    .OrderByDescending(item => item.UpdatedAt)
                    .ToList();

                var totalCountSearch = filteredItems.Count;
                var totalPagesSearch = (int)Math.Ceiling((double)totalCountSearch / pageSize);

                if (totalCountSearch == 0)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "No categories found.",
                        Data = new
                        {
                            Items = new List<object>(),
                            TotalPages = 1,
                            CurrentPage = pageNumber
                        }
                    };
                }

                var pagedSearchItems = filteredItems
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new ServiceResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Search results retrieved successfully.",
                    Data = new
                    {
                        Items = pagedSearchItems,
                        TotalPages = totalPagesSearch,
                        CurrentPage = pageNumber
                    }
                };

            }

            // Buradan aşağısı ise normal şartlardaki arama.

            var totalCountNormal = await query.CountAsync();
            var totalPagesNormal = (int)Math.Ceiling((double)totalCountNormal / pageSize);

            var pagedEncryptedItems = await query
                .OrderByDescending(item => item.UpdatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(item => new
                {
                    item.Id,
                    item.EncryptedName,
                    item.UpdatedAt,
                    item.CreatedAt
                })
                .ToListAsync();

            if (!pagedEncryptedItems.Any())
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "No categories found.",
                    Data = new
                    {
                        Items = new List<object>(),
                        TotalPages = totalPagesNormal,
                        CurrentPage = pageNumber
                    }
                };
            }

            var pagedDecryptedItems = pagedEncryptedItems.Select(item => new
            {
                item.Id,
                CategoryName = _encryptionService.Decrypt(item.EncryptedName),
                item.UpdatedAt,
                item.CreatedAt
            }).ToList();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Categories successfully retrieved.",
                Data = new
                {
                    Items = pagedDecryptedItems,
                    TotalPages = totalPagesNormal,
                    CurrentPage = pageNumber
                }
            };

        }

        public async Task<ServiceResult> GetCategoryByIdAsync(Guid userId, Guid categoryId)
        {

            var category = await _context.Categories
                .AsNoTracking()
                .Where(category => category.Id == categoryId && category.UserId == userId)
                .Select(category => new
                {
                    category.Id,
                    category.EncryptedName
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Category not found."
                };
            }

            var decryptedCategory = new
            {
                Id = category.Id,
                CategoryName = _encryptionService.Decrypt(category.EncryptedName)
            };

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category retrieved successfully.",
                Data = decryptedCategory
            };

        }

        public async Task<ServiceResult> DeleteCategoryAsync(Guid userId, Guid categoryId)
        {

            // Transaction işlemi ekledim.
            // 2 farklı db işlemi var ve savechanges kullanmak istemedim. Bütün lsiteyi rame çekmeye gerek yok.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                await _context.VaultItems
                .Where(item => item.CategoryId == categoryId && item.UserId == userId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.CategoryId, (Guid?)null));

                var affectedRows = await _context.Categories
                    .Where(category => category.Id == categoryId && category.UserId == userId)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0)
                {

                    _logger.LogWarning("Category deletion failed. Category not found or unauthorized. CategoryId: {CategoryId}, UserId: {UserId}", categoryId, userId);

                    return new ServiceResult
                    {
                        IsSuccess = false,
                        StatusCode = 404,
                        Message = "Category not found."
                    };

                }

                await transaction.CommitAsync();

                _logger.LogInformation("Category deleted and vault items detached successfully. CategoryId: {CategoryId}, UserId: {UserId}", categoryId, userId);

                return new ServiceResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Category deleted successfully."
                };

            }
            catch (Exception error)
            {
                await transaction.RollbackAsync();
                _logger.LogError(error, "Transaction failed while deleting category. CategoryId: {CategoryId}, UserId: {UserId}", categoryId, userId);
                throw;
            }
           
        }

        public async Task<ServiceResult> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdateCategoryDto updateCategoryDto)
        {

            var encryptedName = _encryptionService.Encrypt(updateCategoryDto.CategoryName.Trim());

            var affectedRows = await _context.Categories
                .Where(item => item.Id == categoryId && item.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.EncryptedName, encryptedName)
                    .SetProperty(item => item.UpdatedAt, DateTime.UtcNow));

            if (affectedRows == 0)
            {

                _logger.LogWarning("Category update failed. Category not found or unauthorized. CategoryId: {CategoryId}, UserId: {UserId}", categoryId, userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Category not found."
                };
            }

            _logger.LogInformation("Category updated successfully. CategoryId: {CategoryId}, UserId: {UserId}", categoryId, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category updated successfully."
            };

        }

        public async Task<ServiceResult> GetAllCategoriesAsync(Guid userId)
        {

            var encryptedCategories = await _context.Categories
                .AsNoTracking()
                .Where(item => item.UserId == userId)
                .Select(item => new
                {
                    item.Id,
                    item.EncryptedName
                })
                .ToListAsync();

            var decryptedCategories = encryptedCategories
                .Select(item => new
                {
                    Id = item.Id,
                    CategoryName = _encryptionService.Decrypt(item.EncryptedName)
                })
                .OrderBy(item => item.CategoryName) 
                .ToList();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Categories retrieved successfully.",
                Data = decryptedCategories
            };

        }
    }
}
