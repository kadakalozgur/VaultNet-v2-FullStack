using Microsoft.AspNetCore.DataProtection;
using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class DataProtectionEncryptionService : IEncryptionService
    {

        private readonly IDataProtector _dataProtector;

        public DataProtectionEncryptionService(IDataProtector dataProtector)
        {
            _dataProtector = dataProtector.CreateProtector("VaultNet.VaultItemProtector");
        }

        public string Encrypt(string plainText)
        {
            return _dataProtector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            return _dataProtector.Unprotect(cipherText);
        }

    }
}
