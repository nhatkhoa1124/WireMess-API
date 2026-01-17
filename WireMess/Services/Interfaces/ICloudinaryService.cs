using CloudinaryDotNet.Actions;

namespace WireMess.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadAvatarAsync(IFormFile file, string userId);
        Task<DeletionResult> DeleteAvatarAsync(string publicId);
        Task<string> UploadFromUrlAsync(string imageUrl, string userId);
    }
}
