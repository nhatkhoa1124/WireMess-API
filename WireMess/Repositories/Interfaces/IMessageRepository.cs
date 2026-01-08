using WireMess.Models.Entities;

namespace WireMess.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();
        Task<IEnumerable<Message>> GetRecent(int take);
        Task<Message?> GetByIdAsync(int id);
        Task<Message> CreateAsync(Message message);
        Task<Message> CreateWithAttachmentsAsync(Message message, IEnumerable<IFormFile> files);
        Task<Message> UpdateAsync(Message message);
        Task<Message?> DeleteAsync(int id);

    }
}
