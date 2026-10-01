namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IPasswordHashService
    {

        string HashPassword(string password);

        bool VerifyPassword(string password,string passwordHash);

    }
}
