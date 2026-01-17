using WireMess.Models.DTOs.Request.Message;
using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;

namespace WireMess.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDto>> CreateAsync(MessageCreateDto request, int senderId);
        Task<MessageDto?> GetByIdAsync(int id);
        Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId, int page = 1, int pageSize = 50);
        Task<MessageDto?> UpdateByIdAsync(int id, MessageUpdateDto request);
        Task<bool> DeleteByIdAsync(int id);
    }
}
