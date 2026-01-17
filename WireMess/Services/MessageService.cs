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
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IMessageRepository messageRepository, 
            IConversationRepository conversationRepository,
            ICloudinaryService cloudinaryService,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<List<MessageDto>> CreateAsync(MessageCreateDto request, int senderId)
        {
            try
            {
                if (!request.IsValid())
                    throw new ArgumentException("Message must contain either text or an attachment");

                var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId);
                if (conversation == null)
                {
                    _logger.LogWarning("Conversation ID {conversationId} not found", request.ConversationId);
                    return null;
                }

                var createdMessages = new List<MessageDto>();
                var hasContent = !string.IsNullOrWhiteSpace(request.Content);
                var hasAttachment = request.Attachment != null;
                
                if(hasContent)
                {
                    var textMessage = new Message
                    {
                        Content = request.Content,
                        SenderId = senderId,
                        ConversationId = request.ConversationId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var createdTextMessage = await _messageRepository.CreateAsync(textMessage);
                    if (createdTextMessage != null)
                    {
                        createdMessages.Add(createdTextMessage.MapMessageToDto());
                    }
                }

                if (hasAttachment)
                {
                    var attachmentMessage = new Message
                    {
                        Content = null,
                        SenderId = senderId,
                        ConversationId = request.ConversationId,
                        CreatedAt = DateTime.UtcNow.AddMilliseconds(1),
                        UpdatedAt = DateTime.UtcNow.AddMilliseconds(1)
                    };

                    var createdAttachmentMessage = await _messageRepository.CreateAsync(attachmentMessage);
                    if (createdAttachmentMessage == null)
                    {
                        _logger.LogError("Failed to create attachment message");
                        return createdMessages;
                    }
                    try
                    {
                        var uploadResult = await _cloudinaryService.UploadAttachmentAsync(
                            request.Attachment,
                            createdAttachmentMessage.Id
                            );
                        var attachment = new Attachment()
                        {
                            FileName = request.Attachment.FileName,
                            FileSize = request.Attachment.Length,
                            StoragePath = uploadResult.SecureUrl.ToString(),
                            PublicId = uploadResult.PublicId,
                            FileType = request.Attachment.ContentType,
                            MessageId = createdAttachmentMessage.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _messageRepository.AddAttachmentAsync(createdAttachmentMessage.Id, attachment);

                        var messageWithAttachment = await _messageRepository.GetByIdAsync(createdAttachmentMessage.Id);
                        createdMessages.Add(messageWithAttachment.MapMessageToDto());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload attachment for message {id}", createdAttachmentMessage.Id);
                        await _messageRepository.DeleteByIdAsync(createdAttachmentMessage.Id);
                    }
                }

                conversation.LastMessageAt = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversation);

                return createdMessages;
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
                return await _messageRepository.DeleteByIdAsync(id);
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
