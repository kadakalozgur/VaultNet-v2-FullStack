namespace VaultNetAPIv2.Services.Interfaces
{
    public interface IEncryptionService
    {

        string Encrypt(string plainText);

        string Decrypt(string cipherText);

    }
}
