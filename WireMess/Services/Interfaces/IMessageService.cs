using WireMess.Models.DTOs.Request.Message;
using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;

namespace WireMess.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllAsync();
        Task<MessageDto> GetByIdAsync();
        Task<MessageDto> UpdateByIdAsync(int id);
        Task<MessageDto> CreateAsync(int senderId, MessageCreateDto request);
        Task<bool> DeleteByIdAsync();
    }
}
