using WireMess.Models.Entities;

namespace WireMess.Repositories.Interfaces
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetAllAsync();
        Task<IEnumerable<Conversation>> GetRecent(int take);
        Task<Conversation?> GetByIdAsync(int id);
        Task<Conversation> CreateAsync(Conversation conversation);
        Task<Conversation> UpdateAsync(Conversation Conversation);
        Task<Conversation?> DeleteAsync(int id);
    }
}
