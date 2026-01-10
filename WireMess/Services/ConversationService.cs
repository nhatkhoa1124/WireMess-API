using WireMess.Models.DTOs.Request.Conversation;
using WireMess.Models.DTOs.Response.Conversation;
using WireMess.Models.Entities;
using WireMess.Models.Enums;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(IConversationRepository conversationRepository, ILogger<ConversationService> logger)
        {
            _conversationRepository = conversationRepository;
            _logger = logger;
        }

        public async Task<ConversationDto> CreateDirectAsync()
        {
            try
            {
                var newConversation = new Conversation
                {
                    ConversationName = null,
                    TypeId = (int)ConversationTypeEnum.Direct,
                    AvatarUrl = null,
                    LastMessageAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdConversation = await _conversationRepository.CreateAsync(newConversation);
                if (createdConversation == null)
                    throw new Exception("Error creating direct conversation async");

                return createdConversation.MapConversationToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating direct conversation");
                throw;
            }
        }

        public async Task<ConversationDto> CreateGroupAsync(ConversationCreateUpdateDto request)
        {
            try
            {
                var newConversation = new Conversation
                {
                    ConversationName = request.ConversationName,
                    TypeId = (int)ConversationTypeEnum.Group,
                    AvatarUrl = request.AvatarUrl,
                    LastMessageAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdConversation = await _conversationRepository.CreateAsync(newConversation);
                if (createdConversation == null)
                    throw new Exception("Error creating direct conversation async");

                return createdConversation.MapConversationToDto();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating direct conversation");
                throw;
            }
        }

        public async Task<ConversationDto> DeleteByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(id);
                if (conversation == null)
                    throw new ArgumentException($"Conversation not found with ID: {id}");
                var deleted = await _conversationRepository.DeleteAsync(id);
                if (deleted == null)
                    throw new InvalidOperationException("Error deleting conversation");

                return deleted.MapConversationToDto();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting conversation");
                throw;
            }
        }

        public async Task<IEnumerable<ConversationDto>> GetAllAsync()
        {
            try
            {
                var conversations = await _conversationRepository.GetAllAsync();
                if (conversations == null)
                    return Enumerable.Empty<ConversationDto>();
                return conversations.Select(c => c.MapConversationToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all conversations");
                throw;
            }
        }

        public async Task<ConversationDto> GetByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(id);
                if (conversation == null)
                    throw new ArgumentException($"Conversation not found with ID: {id}");
                return conversation.MapConversationToDto();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation with ID: {id}", id);
                throw;
            }
        }

        public async Task<ConversationDto> UpdateGroupProfileByIdAsync(int id, ConversationCreateUpdateDto request)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(id);
                if (conversation == null)
                    throw new ArgumentException($"Conversation not found with ID: {id}");
                if(conversation.TypeId == (int) ConversationTypeEnum.Direct)
                {
                    _logger.LogWarning("Cannot update profile of direct conversation");
                    throw new ArgumentException($"Conversation update failed with ID: {id}");
                }

                conversation.ConversationName = request.ConversationName;
                conversation.AvatarUrl = request.AvatarUrl;

                var updatedConversation = await _conversationRepository.UpdateAsync(conversation);
                if (updatedConversation == null)
                    throw new InvalidOperationException("Conversation update failed");

                return updatedConversation.MapConversationToDto();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating group profile");
                throw;
            }
        }
    }
}
