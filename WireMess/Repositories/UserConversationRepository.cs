using WireMess.Data;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WireMess.Repositories
{
    public class UserConversationRepository : IUserConversationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserConversationRepository> _logger;

        public UserConversationRepository(AppDbContext context, ILogger<UserConversationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddUserToConversationAsync(int userId, int conversationId)
        {
            try
            {
                var userExists = await IsUserExistInConversationAsync(userId, conversationId);
                if (userExists)
                {
                    _logger.LogWarning("User already in this conversation");
                    return false;
                }
                var userConversation = new UserConversation
                {
                    UserId = userId,
                    ConversationId = conversationId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.UserConversations.AddAsync(userConversation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new user with ID: {userId} to "
                    + "conversation with ID: {conversationId}", userId, conversationId);
                throw;
            }
        }

        public async Task<UserConversation> CreateAsync(UserConversation userConversation)
        {
            try
            {
                await _context.UserConversations.AddAsync(userConversation);
                await _context.SaveChangesAsync();
                return userConversation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user conversation");
                throw;
            }
        }

        public async Task<bool> DeleteAllByConversationId(int conversationId)
        {
            try
            {
                var userConversations = await _context.UserConversations
                    .Where(uc => uc.ConversationId == conversationId)
                    .ToListAsync();
                if (!userConversations.Any())
                {
                    _logger.LogWarning("No conversation with ID: {conversationId} found", conversationId);
                    return false;
                }

                foreach(var uc in userConversations)
                {
                    uc.IsDeleted = true;
                    uc.DeletedAt = DateTime.UtcNow;
                }
                _context.UserConversations.UpdateRange(userConversations);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting all conversations with ID: {conversationId}", conversationId);
                throw;
            }
        }

        public async Task<bool> DeleteByKeyAsync(int userId, int conversationId)
        {
            try
            {
                var userConversation = await _context.UserConversations
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);
                if (userConversation == null)
                    return false;

                userConversation.IsDeleted = true;
                userConversation.DeletedAt = DateTime.UtcNow;

                _context.UserConversations.Update(userConversation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user conversation with "
                    + "UserID: {userId}, ConversationID: {conversationId}", userId, conversationId);
                throw;
            }
        }

        public async Task<UserConversation?> GetByKeyAsync(int userId, int conversationId)
        {
            try
            {
                return await _context.UserConversations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversation with "
                    + "UserID: {userId}, ConversationID: {conversationId}", userId, conversationId);
                throw;
            }
        }

        public async Task<bool> IsUserExistInConversationAsync(int userId, int conversationId)
        {
            try
            {
                return await _context.UserConversations
                    .AsNoTracking()
                    .AnyAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error checking if user is conversation with "
                    + "UserID: {userId}, ConversationID: {conversationId}", userId, conversationId);
                throw;
            }
        }

        public async Task<bool> RemoveUserFromConversationAsync(int userId, int conversationId)
        {
            try
            {
                var userConversation = await _context.UserConversations
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);
                if(userConversation == null)
                {
                    _logger.LogWarning("User does not exist in this conversation");
                    return false;
                }

                _context.UserConversations.Remove(userConversation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new user with ID: {userId} to "
                    + "conversation with ID: {conversationId}", userId, conversationId);
                throw;
            }
        }
    }
}
