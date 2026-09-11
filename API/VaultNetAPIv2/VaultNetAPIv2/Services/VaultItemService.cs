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

    }
}
