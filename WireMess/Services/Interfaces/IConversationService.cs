using WireMess.Models.DTOs.Request.Conversation;
using WireMess.Models.DTOs.Response.Conversation;

namespace WireMess.Services.Interfaces
{
    public interface IConversationService
    {
        Task<IEnumerable<ConversationDto>> GetAllAsync();
        Task<ConversationDto> GetByIdAsync(int id);
        Task<ConversationDto> CreateAsync(ConversationCreateUpdateDto request);
        Task<ConversationDto> UpdateGroupProfileByIdAsync(int id, ConversationCreateUpdateDto request);
        Task<bool> DeleteByIdAsync(int id);
    }
}
