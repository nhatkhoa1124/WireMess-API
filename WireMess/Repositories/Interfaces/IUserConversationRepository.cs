using WireMess.Models.Entities;

namespace WireMess.Repositories.Interfaces
{
    public interface IUserConversationRepository
    {
        Task<UserConversation?> GetByKeyAsync(int userId, int conversationId);
        Task<IEnumerable<Conversation>> GetAllByUserIdAsync(int userId);
        Task<UserConversation> CreateAsync(UserConversation userConversation);
        Task<bool> DeleteByKeyAsync(int userId, int conversationId);
        Task<bool> DeleteAllByConversationId(int conversationId);
        Task<bool> AddUserToConversationAsync(int userId, int conversationId);
        Task<bool> RemoveUserFromConversationAsync(int userId, int conversationId);
        Task<bool> IsUserExistInConversationAsync(int userId, int conversationId);

    }
}
