using VaultNetAPIv2.Models;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
