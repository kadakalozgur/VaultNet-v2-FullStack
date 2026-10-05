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
        private readonly IPasswordHashService _passwordHashService;

        public UserProfileService(AppDbContext context, IStorageService storageService, IPasswordHashService passwordHashService)
        {
            _context = context;
            _storageService = storageService;
            _passwordHashService = passwordHashService;
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

            var user = await _context.Users
                .AsNoTracking()
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => new { user.ProfilePictureUrl })
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

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await _storageService.DeleteFileAsync(user.ProfilePictureUrl);
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

            var affectedRows = await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(user => user.Email, newEmail)
                    .SetProperty(user => user.SecurityStamp, Guid.NewGuid()));

            if (affectedRows == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "No such user was found."
                };
            }

            await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null)
                .ExecuteUpdateAsync(setter => setter.SetProperty(token => token.RevokedAt, DateTime.UtcNow));

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "The email address has been successfully updated."
            };

        }

        public async Task<ServiceResult> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {        
            
            if(changePasswordDto.CurrentPassword == changePasswordDto.NewPassword)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Your new password cannot be the same as your old password."
                };

            }

            var userPasswordHash = await _context.Users
                .AsNoTracking()
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => user.PasswordHash)
                .FirstOrDefaultAsync();

            if(userPasswordHash == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "No such user was found."
                };
            }

            bool isCurrentPasswordVerify = _passwordHashService.VerifyPassword(changePasswordDto.CurrentPassword, userPasswordHash);

            if (!isCurrentPasswordVerify)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "The current password you entered is incorrect."
                };
            }

            var newPasswordHash = _passwordHashService.HashPassword(changePasswordDto.NewPassword);

            await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(user => user.PasswordHash, newPasswordHash)
                    .SetProperty(user => user.SecurityStamp, Guid.NewGuid()));

            await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null)
                .ExecuteUpdateAsync(setter => setter.SetProperty(token => token.RevokedAt, DateTime.UtcNow));

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Password has been successfully updated."
            };

        }

        public async Task<ServiceResult> DeleteAccountAsync(Guid userId, DeleteAccountDto deleteAccountDto)
        {

            var user = await _context.Users
                .AsNoTracking()
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => new
                {
                    user.PasswordHash,
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

            bool isCurrentPasswordVerify = _passwordHashService.VerifyPassword(deleteAccountDto.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordVerify)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "The current password you entered is incorrect."
                };
            }

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await _storageService.DeleteFileAsync(user.ProfilePictureUrl);
            }

            await _context.Users
                .Where(user => user.Id == userId)
                .ExecuteDeleteAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Your account and all associated data have been permanently deleted."
            };

        }

        public async Task<ServiceResult> RemoveProfilePictureAsync(Guid userId)
        {

            var user = await _context.Users
                .AsNoTracking()
                .Where(user => user.Id == userId && user.IsActive)
                .Select(user => new { user.ProfilePictureUrl })
                .FirstOrDefaultAsync();

            if(user == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "No such user was found."
                };
            }

            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "User does not have a profile picture to remove."
                };
            }

            await _context.Users
                .Where(user => user.Id == userId && user.IsActive)
                .ExecuteUpdateAsync(setter => setter.SetProperty(user => user.ProfilePictureUrl, (string?)null));

            await _storageService.DeleteFileAsync(user.ProfilePictureUrl);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Profile picture removed successfully."
            };

        }
    }
}
