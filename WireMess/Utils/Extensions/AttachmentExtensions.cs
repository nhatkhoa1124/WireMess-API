using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;

namespace WireMess.Utils.Extensions
{
    public static class AttachmentExtensions
    {
        public static AttachmentDto MapAttachmentToDto(this Attachment attachment)
        {
            return new AttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                StoragePath = attachment.StoragePath,
                PublicId = attachment.PublicId,
                FileType = attachment.FileType
            };
        }
    }
}
