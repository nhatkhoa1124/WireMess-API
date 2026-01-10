using WireMess.Models.DTOs.Request.Message;
using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;

namespace WireMess.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public Task<MessageDto> CreateAsync(int senderId, MessageCreateDto request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MessageDto> GetByIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MessageDto> UpdateByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
