using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using VaultNetAPIv2.Data;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Models;
using VaultNetAPIv2.Results;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class VaultItemService : IVaultItemService
    {

        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<VaultItemService> _logger;

        public VaultItemService(AppDbContext context, IEncryptionService encryptionService, ILogger<VaultItemService> logger)
        {
            _context = context;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<ServiceResult> AddItemAsync(Guid userId, AddVaultItemDto addVaultItemDto)
        {

            var currentItemCount = await _context.VaultItems.CountAsync(item => item.UserId == userId);

            if (currentItemCount >= 50)
            {

                _logger.LogWarning("Vault item creation failed. User reached maximum storage limit (50). UserId: {UserId}", userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "You have reached the maximum limit of 50 items."
                };

            }

            if (addVaultItemDto.CategoryId != null && !await _context.Categories.AnyAsync(category => category.Id == addVaultItemDto.CategoryId && category.UserId == userId))
            {
                _logger.LogWarning("Vault item creation rejected. Invalid or unauthorized category. CategoryId: {CategoryId}, UserId: {UserId}", addVaultItemDto.CategoryId, userId);

                return new ServiceResult { IsSuccess = false, StatusCode = 400, Message = "Invalid category." };
            }

            var newVaultItem = new VaultItem
            {
                UserId = userId,
                EncryptedTitle = _encryptionService.Encrypt(addVaultItemDto.Title.Trim()),
                EncryptedUsernameOrEmail = _encryptionService.Encrypt(addVaultItemDto.UsernameOrEmail.Trim()),
                EncryptedPassword = _encryptionService.Encrypt(addVaultItemDto.Password),
                EncryptedDomain = addVaultItemDto.Domain.Trim() != null ? _encryptionService.Encrypt(addVaultItemDto.Domain.Trim()) : null,
                CategoryId = addVaultItemDto.CategoryId
            };

            _context.VaultItems.Add(newVaultItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vault item added successfully. ItemId: {ItemId}, UserId: {UserId}", newVaultItem.Id, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Vault item added successfully."
            };

        }

        // Sıkı sıkıya bağımlı yaptım. Benim projemde bu değerde olmasını istiyorum.
        private const int pageSize = 6;
        public async Task<ServiceResult> GetItemsAsync(Guid userId, int pageNumber, string? searchQuery = null, bool? isFavorite = null, Guid? categoryId = null)
        {

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = _context.VaultItems
                .AsNoTracking()
                .Where(item => item.UserId == userId);

            if (isFavorite == true)
            {
                query = query.Where(item => item.IsFavorite == true);
            }

            if(categoryId != null)
            {
                query = query.Where(item => item.CategoryId == categoryId);
            }

            // Bu if bloğu search işlemi esnasında çalışıyor.
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {

                var lowerSearchQuery = searchQuery.Trim().ToLower();

                var allEncryptedItems = await query
                    .Select(item => new 
                    { item.Id, 
                      item.EncryptedTitle,
                      item.EncryptedDomain,
                      item.UpdatedAt,
                      item.CreatedAt,
                      item.IsFavorite
                    })
                    .ToListAsync();

                var filteredItems = allEncryptedItems
                    .Select(item => new
                    {
                        item.Id,
                        Title = _encryptionService.Decrypt(item.EncryptedTitle),
                        Domain = item.EncryptedDomain != null ? _encryptionService.Decrypt(item.EncryptedDomain) : null,
                        item.UpdatedAt,
                        item.CreatedAt,
                        item.IsFavorite
                    })
                    .Where(item => item.Title.ToLower().Contains(lowerSearchQuery))
                    .OrderByDescending(item => item.UpdatedAt)
                    .ToList();

                var totalCountSearch = filteredItems.Count;
                var totalPagesSearch = (int)Math.Ceiling((double)totalCountSearch / pageSize);

                if(totalCountSearch == 0)
                {
                    return new ServiceResult
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "No vault items found.",
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
                    item.EncryptedTitle, 
                    item.EncryptedDomain,
                    item.UpdatedAt,
                    item.CreatedAt,
                    item.IsFavorite
                })
                .ToListAsync();

            
            if (!pagedEncryptedItems.Any())
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "No vault items found.",
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
                Title = _encryptionService.Decrypt(item.EncryptedTitle),
                Domain = item.EncryptedDomain != null ? _encryptionService.Decrypt(item.EncryptedDomain) : null,
                item.UpdatedAt,
                item.CreatedAt,
                item.IsFavorite
            }).ToList();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Vault items successfully retrieved.",
                Data = new
                {
                    Items = pagedDecryptedItems,
                    TotalPages = totalPagesNormal,
                    CurrentPage = pageNumber
                }
            };

        }

        public async Task<ServiceResult> GetItemByIdAsync(Guid userId, Guid itemId)
        {

            var item = await _context.VaultItems
                .AsNoTracking()
                .Where(item => item.Id == itemId && item.UserId == userId)
                .Select(item => new
                {
                    item.EncryptedTitle,
                    item.EncryptedUsernameOrEmail,
                    item.EncryptedPassword,
                    item.EncryptedDomain,
                    item.CategoryId,
                    item.CreatedAt,
                    item.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if(item == null)
            {

                _logger.LogWarning("Vault item retrieval failed. Item not found or unauthorized. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            var decryptedItem = new
            {
                Id = itemId,
                Title = _encryptionService.Decrypt(item.EncryptedTitle),
                UsernameOrEmail = _encryptionService.Decrypt(item.EncryptedUsernameOrEmail),
                Password = _encryptionService.Decrypt(item.EncryptedPassword),
                Domain = item.EncryptedDomain != null ? _encryptionService.Decrypt(item.EncryptedDomain) : null,
                CategoryId = item.CategoryId,
                item.CreatedAt,
                item.UpdatedAt
            };

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Item retrieved successfully.",
                Data = decryptedItem
            };

        }

        public async Task<ServiceResult> DeleteItemAsync(Guid userId, Guid itemId)
        {

            var affectedRows = await _context.VaultItems
                .Where(item => item.Id == itemId && item.UserId == userId)
                .ExecuteDeleteAsync();

            if(affectedRows == 0)
            {

                _logger.LogWarning("Vault item deletion failed. Item not found or unauthorized. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            _logger.LogInformation("Vault item deleted successfully. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Vault item deleted successfully."
            };

        }

        public async Task<ServiceResult> UpdateItemAsync(Guid userId, Guid itemId, UpdateVaultItemDto updateVaultItemDto)
        {

            if (updateVaultItemDto.CategoryId != null && !await _context.Categories.AnyAsync(category => category.Id == updateVaultItemDto.CategoryId && category.UserId == userId))
            {
                _logger.LogWarning("Vault item update rejected. Invalid or unauthorized category. CategoryId: {CategoryId}, UserId: {UserId}", updateVaultItemDto.CategoryId, userId);

                return new ServiceResult { IsSuccess = false, StatusCode = 400, Message = "Invalid category." };
            }

            var encryptedTitle = _encryptionService.Encrypt(updateVaultItemDto.Title.Trim());
            var encryptedUsernameOrEmail = _encryptionService.Encrypt(updateVaultItemDto.UsernameOrEmail.Trim());
            var encryptedPassword = _encryptionService.Encrypt(updateVaultItemDto.Password);
            var encryptedDomain = updateVaultItemDto.Domain.Trim() != null ? _encryptionService.Encrypt(updateVaultItemDto.Domain.Trim()) : null;
        
            var affectedRows = await _context.VaultItems
                .Where(item => item.Id == itemId && item.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.EncryptedTitle, encryptedTitle)
                .SetProperty(item => item.EncryptedUsernameOrEmail, encryptedUsernameOrEmail)
                .SetProperty(item => item.EncryptedPassword, encryptedPassword)
                .SetProperty(item => item.EncryptedDomain, encryptedDomain)
                .SetProperty(item => item.CategoryId, updateVaultItemDto.CategoryId)
                .SetProperty(item => item.UpdatedAt, DateTime.UtcNow));

            if (affectedRows == 0)
            {

                _logger.LogWarning("Vault item update failed. Item not found or unauthorized. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            _logger.LogInformation("Vault item updated successfully. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Vault item updated successfully."
            };

        }

        public async Task<ServiceResult> ToggleFavoriteItemAsync(Guid userId, Guid itemId)
        {

            var affectedRows = await _context.VaultItems
                .Where(item => item.Id == itemId && item.UserId == userId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(item => item.IsFavorite, item => !item.IsFavorite)
                );

            if (affectedRows == 0)
            {

                _logger.LogWarning("Toggle favorite status failed. Item not found or unauthorized. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            _logger.LogInformation("Vault item favorite status toggled successfully. ItemId: {ItemId}, UserId: {UserId}", itemId, userId);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Favorite status updated successfully."
            };

        }

        public async Task<ServiceResult> GetDashboardStatsAsync(Guid userId, bool isCachedCounts = false)
        {

            int totalCount = 0;
            int favoriteCount = 0;

            // Eğer count değerleri UI'da cachlendiyse boşuna db'yi yormamak için.
            if (!isCachedCounts)
            {

                var counts = await _context.VaultItems
                .AsNoTracking()
                .Where(item => item.UserId == userId)
                .GroupBy(item => 1)
                .Select(userItems => new
                {
                    Total = userItems.Count(),
                    Favorites = userItems.Count(item => item.IsFavorite)
                })
                .FirstOrDefaultAsync();

                totalCount = counts?.Total ?? 0;
                favoriteCount = counts?.Favorites ?? 0;

            }
            
            var recentEncryptedItems = await _context.VaultItems
                .AsNoTracking()
                .Where(item => item.UserId == userId)
                .OrderByDescending(item => item.CreatedAt)
                .Take(5)
                .Select(item => new
                {
                    item.Id,
                    item.EncryptedTitle,
                    item.EncryptedDomain,
                    item.CreatedAt
                })
                .ToListAsync();

            var recentDecryptedItems = recentEncryptedItems.Select(item => new
            {
                item.Id,
                Title = _encryptionService.Decrypt(item.EncryptedTitle),
                Domain = item.EncryptedDomain != null ? _encryptionService.Decrypt(item.EncryptedDomain) : null,
                item.CreatedAt
            })
            .ToList();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Dashboard statistics retrieved successfully.",
                Data = new
                {
                    TotalCount = totalCount,
                    FavoriteCount = favoriteCount,
                    RecentItems = recentDecryptedItems
                }
            };

        }
    }
}
