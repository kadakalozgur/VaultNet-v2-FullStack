using Microsoft.EntityFrameworkCore;
using VaultNetAPIv2.Data;
using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Models;
using VaultNetAPIv2.Results;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class AuthService : IAuthService
    {

        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHashService _passwordHashService;

        public AuthService(AppDbContext context, ITokenService tokenService, IPasswordHashService passwordHashService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHashService = passwordHashService;
        }

        public async Task<ServiceResult> RegisterAsync(RegisterDto registerDto)
        {
            
            var email = registerDto.Email.Trim().ToLower().Replace(" ","");

            bool isUserExist = await _context.Users.AnyAsync(user => user.Email == email);

            if (isUserExist)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    Message = "This email address is already in use."
                };
            }

            var newUser = new User
            {
                Email = email,
                PasswordHash = _passwordHashService.HashPassword(registerDto.Password)
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 201,
                Message = "Registration completed successfully."
            };

        }

        public async Task<ServiceResult> LoginAsync(LoginDto loginDto)
        {

            var email = loginDto.Email.Trim().ToLower().Replace(" ", "");

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email);

            if(user == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "The email address or password is incorrect."
                };
            }

            bool isPasswordCorret = _passwordHashService.VerifyPassword(loginDto.Password,user.PasswordHash);

            if (!isPasswordCorret)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "The email address or password is incorrect."
                };
            }

            if (!user.IsActive)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    Message = "Your account has been deactivated."
                };
            }

            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            var refreshTokenDbItem = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null
            };

            _context.RefreshTokens.Add(refreshTokenDbItem);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                Message = "The login process was successfully completed."
            };

        }

        public async Task<ServiceResult> LogoutAsync(string? refreshToken)
        {

            if (string.IsNullOrEmpty(refreshToken))
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "No session found to close.",
                    StatusCode = 400,
                    Data = null
                };
            }

            var tokenHash = _tokenService.HashRefreshToken(refreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.RevokedAt == null);

            if (storedToken != null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "The logout process was successfully completed."
            };

        }
    }
}
