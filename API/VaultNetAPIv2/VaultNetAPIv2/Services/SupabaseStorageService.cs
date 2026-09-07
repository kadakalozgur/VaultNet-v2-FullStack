using Supabase.Interfaces;
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

        // Hazır şablon olarak alındı.
        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return false;
                }

                var uri = new Uri(fileUrl);
                var pathSegments = uri.AbsolutePath.Split('/');

                int publicIndex = Array.IndexOf(pathSegments, "public");

                if (publicIndex == -1 || publicIndex + 2 >= pathSegments.Length)
                {
                    return false;
                }

                string bucketName = pathSegments[publicIndex + 1];
                string filePath = string.Join("/", pathSegments.Skip(publicIndex + 2));

                var response = await _client.Storage
                    .From(bucketName)
                    .Remove(new List<string> { filePath });

                return response != null;

            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
