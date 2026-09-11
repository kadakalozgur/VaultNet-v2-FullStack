using Microsoft.EntityFrameworkCore;
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

        public async Task<ServiceResult> GetItemsAsync(Guid userId, int pageNumber, int pageSize)
        {

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            if(pageSize < 1)
            {
                pageSize = 6;
            }

            var query = _context.VaultItems
                .AsNoTracking()
                .Where(item => item.UserId == userId);    

            var totalCount = await query.CountAsync();

            var encryptedItems = await query
                .OrderByDescending(item => item.UpdatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(item => new
                {
                    item.Id,
                    item.EncryptedTitle, 
                    item.UpdatedAt,
                    item.CreatedAt,
                    item.IsFavorite
                })
                .ToListAsync();

            var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

            if (!encryptedItems.Any())
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "No vault items found.",
                    Data = new
                    {
                        Items = new List<object>(),
                        TotalPages = totalPage,
                        CurrentPage = pageNumber
                    }
                };
            }

            var decryptedItems = encryptedItems.Select(item => new
            {
                item.Id,
                Title = _encryptionService.Decrypt(item.EncryptedTitle),
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
                    Items = decryptedItems,
                    TotalPages = totalPage,
                    CurrentPage = pageNumber
                }
            };

        }

    }
}
