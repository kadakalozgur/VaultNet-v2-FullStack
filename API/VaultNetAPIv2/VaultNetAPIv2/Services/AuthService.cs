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

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
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

            bool isPasswordCorret = BCrypt.Net.BCrypt.Verify(loginDto.Password,user.PasswordHash);

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

            var token = _tokenService.GenerateToken(user);

            return new ServiceResult
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = new
                {
                    Token = token,
                    Id = user.Id,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                },
                Message = "The login process was successfully completed."
            };

        }
    }
}
