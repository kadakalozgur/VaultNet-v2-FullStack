using VaultNetAPIv2.Services.Interfaces;

namespace VaultNetAPIv2.Services
{
    public class SupabaseStorageService : IStorageService
    {

        private readonly Supabase.Client _client;

        public SupabaseStorageService(Supabase.Client client)
        {
            _client = client;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {

            if(file == null || file.Length == 0)
            {
                return null;
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            await _client.Storage.From(folderName).Upload(imageBytes, fileName);

            return _client.Storage.From(folderName).GetPublicUrl(fileName);

        }
    }
}
