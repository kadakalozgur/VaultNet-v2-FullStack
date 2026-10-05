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

            var twoDayAgo = DateTime.UtcNow.AddDays(-2);
            await _context.RefreshTokens
                .Where(token => token.UserId == user.Id && (token.ExpiresAt < twoDayAgo || token.RevokedAt < twoDayAgo))
                .ExecuteDeleteAsync();

            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            var refreshTokenDbItem = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
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

        public async Task<ServiceResult> RefreshTokenAsync(string? refreshToken)
        {

            if (string.IsNullOrEmpty(refreshToken))
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "Session not found or expired."
                };
            }

            var tokenHash = _tokenService.HashRefreshToken(refreshToken);

            var storedToken = await _context.RefreshTokens
                .Include(token => token.User)
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

            if (storedToken == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "Invalid or previously cancelled session"
                };
            }

            // Kullanılmış token gelirse bütün tokenları geçersiz yaptım.
            if (storedToken.RevokedAt != null)
            {

                var activeTokens = await _context.RefreshTokens
                    .Where(t => t.UserId == storedToken.UserId && t.RevokedAt == null)
                    .ToListAsync();

                if (activeTokens.Any())
                {
                    foreach (var activeToken in activeTokens)
                    {
                        activeToken.RevokedAt = DateTime.UtcNow; 
                    }

                    await _context.SaveChangesAsync();
                }

                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "Security alert: Token reuse detected. All active sessions have been terminated."
                };

            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 401,
                    Message = "Your session has expired. Please log in again."
                };
            }

            if (!storedToken.User.IsActive)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    Message = "Your account has been disabled."
                };
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            var twoDayAgo = DateTime.UtcNow.AddDays(-2);
            await _context.RefreshTokens
                .Where(token => token.UserId == storedToken.UserId && (token.ExpiresAt < twoDayAgo || token.RevokedAt < twoDayAgo))
                .ExecuteDeleteAsync();

            var newAccessToken = _tokenService.GenerateToken(storedToken.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

            var newRefreshTokenDbItem = new RefreshToken
            {
                UserId = storedToken.UserId,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                RevokedAt = null
            };

            _context.RefreshTokens.Add(newRefreshTokenDbItem);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                },
                Message = "The session has been successfully renewed."
            };

        }
    }
}
