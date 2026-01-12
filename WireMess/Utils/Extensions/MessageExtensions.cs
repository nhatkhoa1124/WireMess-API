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
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt
            };
        }
    }
}
