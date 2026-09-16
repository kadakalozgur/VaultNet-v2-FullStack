using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IVaultItemService
    {

        Task<ServiceResult> AddItemAsync(Guid userId,AddVaultItemDto addVaultItemDto);

        Task<ServiceResult> GetItemsAsync(Guid userId,int pageNumber, string? searchQuery = null);

        Task<ServiceResult> GetItemByIdAsync(Guid userId, Guid itemId);

        Task<ServiceResult> DeleteItemAsync(Guid userId,Guid itemId);

        Task<ServiceResult> UpdateItemAsync(Guid userId, Guid itemId, UpdateVaultItemDto updateVaultItemDto);

        Task<ServiceResult> ToggleFavoriteItemAsync(Guid userId, Guid itemId);

    }
}
