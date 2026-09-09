using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<ServiceResult> GetUserProfileAsync(Guid userId);

        Task<ServiceResult> UpdateProfilePictureAsync(Guid userId, UpdateProfilePictureDto updateProfilePictureDto);

        Task<ServiceResult> UpdateEmailAsync(Guid userId,UpdateEmailDto updateEmailDto);

        Task<ServiceResult> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);

    }
}
