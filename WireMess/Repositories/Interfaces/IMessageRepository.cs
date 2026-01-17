using WireMess.Models.Entities;

namespace WireMess.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();
        Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId, int page = 1, int pageSize = 50);
        Task<Message?> GetByIdAsync(int id);
        Task<Message?> CreateAsync(Message message);
        Task AddAttachmentAsync(int messageId, Attachment attachment);
        Task<Message> UpdateAsync(Message message);
        Task<bool> DeleteByIdAsync(int id);
        Task<bool> DeleteAllByConversationIdAsync(int conversationId);

    }
}
