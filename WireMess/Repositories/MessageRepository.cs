using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WireMess.Data;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;

namespace WireMess.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MessageRepository> _logger;
        private readonly IFileStorageService _fileStorage;

        public MessageRepository(AppDbContext context, ILogger<MessageRepository> logger, IFileStorageService fileStorage)
        {
            _context = context;
            _logger = logger;
            _fileStorage = fileStorage;
        }

        public async Task<Message?> CreateAsync(Message message)
        {
            try
            {
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                return await _context.Messages
                    .Include(m => m.Sender)
                    .FirstOrDefaultAsync(m => m.Id == message.Id);
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating message: SenderId: {SenderId}, " +
                    "ConversationId: {ConversationId}",
                    message.SenderId, message.ConversationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                var message = await _context.Messages
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (message == null)
                    return false;

                message.DeletedAt = DateTime.UtcNow;
                message.IsDeleted = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message ID {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            try
            {
                return await _context.Messages
                    .AsNoTracking()
                    .OrderByDescending(m => m.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all messages");
                throw;
            }
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Messages
                    .AsNoTracking()
                    .Include(m => m.Sender)
                    .Include(m => m.Conversation)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting message by ID {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetByConversationIdAsync(
            int conversationId,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                return await _context.Messages
                    .Include(m => m.Sender)
                    .Where(m => m.ConversationId == conversationId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages of conversation ID: {conversationId}", conversationId);
                throw;
            }
        }

        public async Task<Message> UpdateAsync(Message message)
        {
            try
            {
                var oldMessage = await _context.Messages.FindAsync(message.Id);
                if (oldMessage == null) throw new KeyNotFoundException($"Message with ID {message.Id} not found");

                oldMessage.Content = message.Content;

                await _context.SaveChangesAsync();
                _context.Entry(oldMessage).State = EntityState.Detached;

                return oldMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating message {messageId}", message.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAllByConversationIdAsync(int conversationId)
        {
            try
            {
                var messages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId)
                    .ToListAsync();
                if (messages.Any())
                {
                    _context.Messages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting messages of conversation ID: {conversationId}", conversationId);
                throw;
            }
        }
    }
}
