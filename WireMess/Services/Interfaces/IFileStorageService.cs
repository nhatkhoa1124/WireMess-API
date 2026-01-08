namespace WireMess.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string fileIdentifier);
        Task<Stream> GetAsync(string fileIdentifier);
    }
}
