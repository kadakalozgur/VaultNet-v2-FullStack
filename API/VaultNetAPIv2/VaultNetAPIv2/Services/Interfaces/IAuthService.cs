using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IAuthService
    {

        Task<ServiceResult> RegisterAsync(RegisterDto registerDto);

        Task<ServiceResult> LoginAsync(LoginDto loginDto);

        ServiceResult Logout();

    }
}
