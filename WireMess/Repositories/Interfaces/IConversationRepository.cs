using WireMess.Models.Entities;

namespace WireMess.Repositories.Interfaces
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetAllAsync();
        Task<IEnumerable<Conversation>> GetRecent(int take);
        Task<Conversation?> GetByIdAsync(int id);
        Task<ConversationType?> GetTypeByNameAsync(string name);
        Task<Conversation> CreateAsync(Conversation conversation);
        Task<Conversation> UpdateAsync(Conversation Conversation);
        Task<Conversation> UpdateLastMessageTimeByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
