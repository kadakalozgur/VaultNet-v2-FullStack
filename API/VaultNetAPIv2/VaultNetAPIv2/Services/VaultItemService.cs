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

        public VaultItemService(AppDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<ServiceResult> AddItemAsync(Guid userId, AddVaultItemDto addVaultItemDto)
        {

            var newVaultItem = new VaultItem
            {
                UserId = userId,
                EncryptedTitle = _encryptionService.Encrypt(addVaultItemDto.Title.Trim()),
                EncryptedUsernameOrEmail = _encryptionService.Encrypt(addVaultItemDto.UsernameOrEmail.Trim()),
                EncryptedPassword = _encryptionService.Encrypt(addVaultItemDto.Password),
                EncryptedDomain = addVaultItemDto.Domain.Trim() != null ? _encryptionService.Encrypt(addVaultItemDto.Domain.Trim()) : null
            };

            _context.VaultItems.Add(newVaultItem);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Vault item added successfully."
            };

        }

        // Sıkı sıkıya bağımlı yaptım. Benim projemde bu değerde olmasını istiyorum.
        private const int pageSize = 6;
        public async Task<ServiceResult> GetItemsAsync(Guid userId, int pageNumber, string? searchQuery = null, bool? isFavorite = null)
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

            // Bu if bloğu search işlemi esnasında çalışıyor.
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {

                var lowerSearchQuery = searchQuery.Trim().ToLower();

                var allEncryptedItems = await query
                    .Select(item => new 
                    { item.Id, 
                      item.EncryptedTitle,
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
                .Select(v => new
                {
                    v.EncryptedTitle,
                    v.EncryptedUsernameOrEmail,
                    v.EncryptedPassword,
                    v.EncryptedDomain,
                    v.CreatedAt,
                    v.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if(item == null)
            {
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
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Vault item deleted successfully."
            };

        }

        public async Task<ServiceResult> UpdateItemAsync(Guid userId, Guid itemId, UpdateVaultItemDto updateVaultItemDto)
        {

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
                .SetProperty(item => item.UpdatedAt, DateTime.UtcNow));

            if (affectedRows == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

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
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Vault item not found."
                };
            }

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Favorite status updated successfully."
            };

        }

        public async Task<ServiceResult> GetDashboardStatsAsync(Guid userId)
        {

            var totalItemCount = await _context.VaultItems
                .AsNoTracking()
                .CountAsync(item => item.UserId == userId);

            var favoriteItemCount = await _context.VaultItems
                .AsNoTracking()
                .CountAsync(item => item.UserId == userId && item.IsFavorite);

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
                    TotalCount = totalItemCount,
                    FavoriteCount = favoriteItemCount,
                    RecentItems = recentDecryptedItems
                }
            };

        }
    }
}
