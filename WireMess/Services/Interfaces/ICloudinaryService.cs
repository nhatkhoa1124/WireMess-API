using CloudinaryDotNet.Actions;

namespace WireMess.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadAvatarAsync(IFormFile file, string userId);
        Task<DeletionResult> DeleteAvatarAsync(string publicId);
        Task<string> UploadFromUrlAsync(string imageUrl, string userId);

        // For message attachments
        Task<RawUploadResult> UploadAttachmentAsync(IFormFile file, int messageId);
        Task<DeletionResult> DeleteAttachmentAsync(string publicId);

    }
}
