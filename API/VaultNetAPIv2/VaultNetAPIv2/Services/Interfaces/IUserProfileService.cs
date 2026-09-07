using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<ServiceResult> GetUserProfileAsync(Guid userId);

        Task<ServiceResult> UpdateProfilePictureAsync(Guid userId, IFormFile photo);

    }
}
