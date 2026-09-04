using VaultNetAPIv2.DTOs;
using VaultNetAPIv2.Results;

namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IAuthServices
    {

        Task<ServiceResult> RegisterAsync(RegisterDto registerDto);

    }
}
