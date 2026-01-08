using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WireMess.Data;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;

namespace WireMess.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConversationRepository> _logger;

        public ConversationRepository(AppDbContext context, ILogger<ConversationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Conversation> CreateAsync(Conversation conversation)
        {
            try
            {
                _context.Conversations
                    .Add(conversation);
                await _context.SaveChangesAsync();
                _context.Entry(conversation).State = EntityState.Detached;

                return conversation;
            }
            catch(DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating conversation: {ConversationId}", conversation.Id);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating conversation");
                throw;
            }
        }

        public async Task<Conversation?> DeleteAsync(int id)
        {
            try
            {
                var conversation = await _context.Conversations.FindAsync(id);
                if (conversation == null) return null;

                conversation.DeletedAt = DateTime.UtcNow;
                conversation.IsDeleted = true;

                await _context.SaveChangesAsync();
                _context.Entry(conversation).State = EntityState.Detached;

                return conversation;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error deleting conversation ID {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Conversation>> GetAllAsync()
        {
            try
            {
                return await _context.Conversations
                    .AsNoTracking()
                    .OrderByDescending(c => c.UpdatedAt)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting all conversations");
                throw;
            }
        }

        public async Task<Conversation?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Conversations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation by ID {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Conversation>> GetRecent(int take)
        {
            try
            {
                return await _context.Conversations
                    .AsNoTracking()
                    .OrderByDescending(c => c.UpdatedAt)
                    .Take(take)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting recent {take} conversations", take);
                throw;
            }
        }

        public async Task<Conversation> UpdateAsync(Conversation conversation)
        {
            try
            {
                var oldConversation = await _context.Conversations.FindAsync(conversation.Id);
                if (oldConversation == null) throw new KeyNotFoundException($"Conversation with ID {conversation.Id} not found");

                oldConversation.ConversationName = conversation.ConversationName;
                oldConversation.AvatarUrl = conversation.AvatarUrl;

                await _context.SaveChangesAsync();
                _context.Entry(oldConversation).State = EntityState.Detached;

                return oldConversation;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating conversation ID {conversationId}", conversation.Id);
                throw;
            }
        }
    }
}
