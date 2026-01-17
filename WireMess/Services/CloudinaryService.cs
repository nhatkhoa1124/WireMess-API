using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using WireMess.Services.Interfaces;
using WireMess.Utils.Settings;

namespace WireMess.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> options, ILogger<CloudinaryService> logger)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
            _logger = logger;
        }

        public async Task<DeletionResult> DeleteAvatarAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                    throw new ArgumentException("Public ID is required");
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image,
                    Invalidate = true
                };
                return await _cloudinary.DestroyAsync(deleteParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar");
                throw;
            }
        }

        public async Task<ImageUploadResult> UploadAvatarAsync(IFormFile file, string userId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file provided");
                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("File size exceeds 5MB limit");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    throw new ArgumentException("Invalid file type. Only images allowed");

                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = $"avatars/{userId}/{Guid.NewGuid()}",
                    Transformation = new Transformation()
                    .Width(400).Height(400).Crop("fill").Gravity("face")
                    .Quality("auto:good"),
                    Format = "webp",
                    Tags = $"avatar,user_{userId},profile_image",
                    Overwrite = false,
                    UseFilename = false
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                    throw new Exception("Cloudinary upload failed");
                return uploadResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading Cloudinary file");
                throw;
            }
        }

        public async Task<string> UploadFromUrlAsync(string imageUrl, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    throw new ArgumentException("Image URL is required");

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageUrl),
                    PublicId = $"avatars/{userId}/{Guid.NewGuid()}",
                    Transformation = new Transformation()
                    .Width(400).Height(400).Crop("fill")
                    .Quality("auto:good"),
                    Format = "webp",
                    Folder = "user_avatars"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error uploading avatar url");
                throw;
            }
        }
    }
}
