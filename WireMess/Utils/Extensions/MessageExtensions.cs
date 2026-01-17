using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;

namespace WireMess.Utils.Extensions
{
    public static class MessageExtensions
    {
        public static MessageDto MapMessageToDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                Attachment = message.Attachment?.MapAttachmentToDto(),
                SenderId = message.SenderId,
                ConversationId = message.ConversationId,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
}
