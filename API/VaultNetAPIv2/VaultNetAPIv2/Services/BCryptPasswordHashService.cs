using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class BCryptPasswordHashService : IPasswordHashService
    {

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
