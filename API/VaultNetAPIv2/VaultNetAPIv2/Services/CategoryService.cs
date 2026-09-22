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
    }
}
