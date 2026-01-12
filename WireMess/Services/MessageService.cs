using WireMess.Models.DTOs.Request.Message;
using WireMess.Models.DTOs.Response.Message;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<MessageDto?> CreateAsync(MessageCreateDto request, int senderId)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId);
                if (conversation == null)
                {
                    _logger.LogWarning("Conversation ID {conversationId} not found", request.ConversationId);
                    return null;
                }

                var newMessage = new Message
                {
                    Content = request.Content,
                    SenderId = senderId,
                    ConversationId = request.ConversationId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdMessage = await _messageRepository.CreateAsync(newMessage);
                if (createdMessage == null)
                {
                    _logger.LogError("Failed to create message");
                    return null;
                }

                conversation.LastMessageAt = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversation);

                return createdMessage.MapMessageToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new message");
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(id);
                if(message == null)
                {
                    _logger.LogWarning("Message ID: {id} not found", id);
                    return false;
                }
                return await _messageRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message by ID: {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<MessageDto>> GetByConversationIdAsync(
            int conversationId,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    _logger.LogWarning("Conversation ID: {conversationId} not found", conversationId);
                    return Enumerable.Empty<MessageDto>();
                }

                var messages = await _messageRepository.GetByConversationIdAsync(
                    conversationId,
                    page,
                    pageSize);

                return messages.Select(m => m.MapMessageToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting message by conversation ID: {conversationId}", conversationId);
                throw;
            }
        }

        public async Task<MessageDto?> GetByIdAsync(int id)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(id);
                if (message == null)
                {
                    _logger.LogWarning("Message ID: {id} not found", id);
                    return null;
                }
                return message.MapMessageToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting message ID: {id}", id);
                throw;
            }
        }

        public async Task<MessageDto?> UpdateByIdAsync(int id, MessageUpdateDto request)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(id);
                if (message == null)
                {
                    _logger.LogWarning("Message ID: {id} not found", id);
                    return null;
                }

                message.Content = request.Content ?? message.Content;
                message.UpdatedAt = DateTime.UtcNow;

                var updatedMessage = await _messageRepository.UpdateAsync(message);
                if (updatedMessage == null)
                {
                    _logger.LogError("Failed to update message ID: {id}", id);
                    return null;
                }
                return updatedMessage.MapMessageToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating message of ID: {id}", id);
                throw;
            }
        }
    }
}
