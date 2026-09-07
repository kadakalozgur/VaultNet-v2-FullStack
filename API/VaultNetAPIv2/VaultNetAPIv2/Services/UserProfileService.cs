using Microsoft.EntityFrameworkCore;
using VaultNetAPIv2.Data;
using VaultNetAPIv2.Results;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class UserProfileService : IUserProfileService
    {

        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;

        public UserProfileService(AppDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<ServiceResult> GetUserProfileAsync(Guid userId)
        {

            var user = await _context.Users.AsNoTracking()
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => new
                {
                    user.Email,
                    user.CreatedAt,
                    user.ProfilePictureUrl
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "No such user was found."
                };
            }

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = user,
                Message = "User information successfully retrieved."
            };

        }

        public async Task<ServiceResult> UpdateProfilePictureAsync(Guid userId, IFormFile photo)
        {

            if (photo == null || photo.Length == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Please select a valid image file."
                };
            }

            bool isUserExist = await _context.Users.AnyAsync(user => user.Id == userId && user.IsActive);

            if (!isUserExist)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "No such user was found."
                };
            }

            var imageUrl = await _storageService.UploadFileAsync(photo, "avatars");

            if (string.IsNullOrEmpty(imageUrl))
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "An error occurred while uploading the file to storage."
                };
            }

            await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.ProfilePictureUrl, imageUrl));

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = new
                {
                    ProfilePictureUrl = imageUrl
                },
                Message = "Profile picture updated successfully."
            };

        }
    }
}
