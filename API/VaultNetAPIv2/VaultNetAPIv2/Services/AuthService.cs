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

        public AuthService(AppDbContext context)
        {
            _context = context;
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
    }
}
