using WireMess.Models.DTOs.Response.Conversation;
using WireMess.Models.DTOs.Response.User;
using WireMess.Models.Entities;

namespace WireMess.Utils.Extensions
{
    public static class ConversationExtensions
    {
        public static ConversationDto MapConversationToDto(this Conversation conversation)
        {
            if (conversation == null) return null;

            return new ConversationDto
            {
                Id = conversation.Id,
                ConversationName = conversation.ConversationName,
                LastMessageAt = conversation.LastMessageAt,
                AvatarUrl = conversation.AvatarUrl
            };
        }
    }
}
