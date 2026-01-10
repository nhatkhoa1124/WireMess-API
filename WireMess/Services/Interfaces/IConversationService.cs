using WireMess.Models.DTOs.Request.Conversation;
using WireMess.Models.DTOs.Response.Conversation;

namespace WireMess.Services.Interfaces
{
    public interface IConversationService
    {
        Task<IEnumerable<ConversationDto>> GetAllAsync();
        Task<ConversationDto> GetByIdAsync(int id);
        Task<ConversationDto> CreateGroupAsync(ConversationCreateUpdateDto request);
        Task<ConversationDto> CreateDirectAsync();
        Task<ConversationDto> UpdateGroupProfileAsync(ConversationCreateUpdateDto request);
        Task<bool> DeleteByIdAsync(int id);
    }
}
