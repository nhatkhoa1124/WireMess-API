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
        private readonly IUserConversationRepository _userConversationRepository;
        private readonly ILogger<ConversationService> _logger;


        public ConversationService(IConversationRepository conversationRepository,
            IUserConversationRepository userConversationRepository,
            ILogger<ConversationService> logger)
        {
            _conversationRepository = conversationRepository;
            _userConversationRepository = userConversationRepository;
            _logger = logger;
        }

        public async Task<bool> AddUserToConversationByIdAsync(int userId, int conversationId)
        {
            try
            {
                var addedUserConversation = await _userConversationRepository.
                    AddUserToConversationAsync(userId,conversationId);
                if(!addedUserConversation)
                {
                    _logger.LogWarning("Error adding user to conversation");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding user with ID: {userId} to conversation ID: {conversationId}",
                  userId, conversationId);
                throw;
            }
        }

        public async Task<ConversationDto> CreateAsync(ConversationCreateUpdateDto request)
        {
            try
            {
                if (request.UserIds == null || !request.UserIds.Any())
                {
                    throw new ArgumentException("At least one user is required to create a conversation");
                }
                bool isDirect = (request == null) ||
                    string.IsNullOrWhiteSpace(request.ConversationName);

                if (isDirect && request.UserIds.Count() != 2)
                    throw new ArgumentException("Direct conversations must have exactly 2 participants");
                if (!isDirect && request.UserIds.Count() < 3)
                    throw new ArgumentException("Group conversations must have at least 3 participants");

                var newConversation = new Conversation
                {
                    ConversationName = isDirect ? null : request.ConversationName,
                    TypeId = isDirect ? (int)ConversationTypeEnum.Direct : (int)ConversationTypeEnum.Group,
                    AvatarUrl = isDirect ? null : request.AvatarUrl,
                    LastMessageAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdConversation = await _conversationRepository.CreateAsync(newConversation);
                if (createdConversation == null)
                    throw new Exception("Error creating direct conversation async");

                foreach (var userId in request.UserIds)
                {
                    var userConvesation = new UserConversation
                    {
                        UserId = userId,
                        ConversationId = createdConversation.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _userConversationRepository.CreateAsync(userConvesation);
                }

                return createdConversation.MapConversationToDto();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating direct conversation");
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(id);
                if (conversation == null)
                    throw new ArgumentException($"Conversation not found with ID: {id}");
                var deletedUserConversation = await _userConversationRepository.DeleteAllByConversationId(id);
                var deletedConversation = await _conversationRepository.DeleteAsync(id);

                if (!deletedConversation || !deletedUserConversation)
                {
                    _logger.LogWarning("Error deleting conversation by ID: {id}", id);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation with ID: {id}", id);
                throw;
            }
        }

        public async Task<bool> RemoveUserFromConversationByIdAsync(int userId, int conversationId)
        {
            try
            {
                var deletedUserConversation = await _userConversationRepository.RemoveUserFromConversationAsync(userId, conversationId);
                if(!deletedUserConversation)
                {
                    _logger.LogWarning("Error removing user from conversation");
                    return false;
                }
                var deletedConversation = await _conversationRepository.DeleteAsync(conversationId);
                if(!deletedConversation)
                {
                    _logger.LogWarning("Error deleting conversation");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error removing user with ID: {userId} from conversation ID: {conversationId}",
                  userId, conversationId);
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
                if (conversation.TypeId == (int)ConversationTypeEnum.Direct)
                {
                    _logger.LogWarning("Cannot update profile of direct conversation");
                    throw new ArgumentException($"Conversation update failed with ID: {id}");
                }

                conversation.ConversationName = request.ConversationName ?? conversation.ConversationName;
                conversation.AvatarUrl = request.AvatarUrl ?? conversation.AvatarUrl;

                var updatedConversation = await _conversationRepository.UpdateAsync(conversation);
                if (updatedConversation == null)
                    throw new InvalidOperationException("Conversation update failed");

                return updatedConversation.MapConversationToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group profile");
                throw;
            }
        }
    }
}
