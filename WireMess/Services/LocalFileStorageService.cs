using WireMess.Services.Interfaces;

namespace WireMess.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _storagePath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public async Task DeleteAsync(string fileIdentifier)
        {
            var filePath = Path.Combine(_storagePath, fileIdentifier);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public Task<Stream> GetAsync(string fileIdentifier)
        {
            var filePath = Path.Combine(_storagePath, fileIdentifier);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Task.FromResult<Stream>(stream);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
    }
}
