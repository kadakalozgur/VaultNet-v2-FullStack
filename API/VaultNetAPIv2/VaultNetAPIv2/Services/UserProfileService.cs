using Microsoft.EntityFrameworkCore;
using VaultNetAPIv2.Data;
using VaultNetAPIv2.DTOs;
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

        public async Task<ServiceResult> UpdateProfilePictureAsync(Guid userId, UpdateProfilePictureDto updateProfilePictureDto)
        {

            var photo = updateProfilePictureDto.Photo;

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

            var oldPictureUrl = await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => user.ProfilePictureUrl)
                .FirstOrDefaultAsync();

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

            if (!string.IsNullOrEmpty(oldPictureUrl))
            {
                await _storageService.DeleteFileAsync(oldPictureUrl);
            }

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

        public async Task<ServiceResult> UpdateEmailAsync(Guid userId, UpdateEmailDto updateEmailDto)
        {

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

            var newEmail = updateEmailDto.NewEmail.Trim().ToLower().Replace(" ", "");

            bool isEmailExist = await _context.Users.AnyAsync(user => user.Email == newEmail);

            if (isEmailExist)
            {

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    Message = "This email is already in use."
                };

            }

            await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.Email, newEmail));

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "The email address has been successfully updated."
            };

        }
    }
}
