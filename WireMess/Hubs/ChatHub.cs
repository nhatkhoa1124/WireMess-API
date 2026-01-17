using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WireMess.Models.DTOs.Request.Message;
using WireMess.Services.Interfaces;

namespace WireMess.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(
            IMessageService messageService,
            IConversationService conversationService,
            ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("nameid")?.Value;
            var username = Context.User?.FindFirst("unique_name")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("User {username} (ID: {userId}) connected with"
                    + "ConnectionId: {ConnectionId}", username, userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("nameid")?.Value;
            var username = Context.User?.FindFirst("unique_name")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("User {username} (ID: {userId}) disconnected", username, userId);
            }
            if (exception != null)
            {
                _logger.LogError(exception, "User disconnected with error");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(string conversationId)
        {
            try
            {
                var userId = Context.User?.FindFirst("nameid")?.Value;
                var username = Context.User?.FindFirst("unique_name")?.Value;

                var conversation = await _conversationService.GetByIdAsync(int.Parse(conversationId));
                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("Error", "Conversation not found");
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
                _logger.LogInformation("User {username} (ID: {userId}) joined conversation {conversationId}"
                    , username, userId, conversationId);

                await Clients.OthersInGroup($"conversation_{conversationId}")
                    .SendAsync("UserJoinedConversation", new
                    {
                        userId,
                        username,
                        conversationId,
                        timestamp = DateTime.UtcNow
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining conversation ID: {conversationId}", conversationId);
                await Clients.Caller.SendAsync("Error", "Failed to join conversation");
            }
        }

        public async Task LeaveConversation(string conversationId)
        {
            try
            {
                var userId = Context.User?.FindFirst("nameid")?.Value;
                var username = Context.User?.FindFirst("unique_name")?.Value;

                var conversation = await _conversationService.GetByIdAsync(int.Parse(conversationId));
                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("Error", "Conversation not found");
                    return;
                }

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
                _logger.LogInformation("User {username} (ID: {userId}) left conversation {conversationId}"
                    , username, userId, conversationId);

                await Clients.OthersInGroup($"conversation_{conversationId}")
                    .SendAsync("UserLeftConversation", new
                    {
                        userId,
                        username,
                        conversationId,
                        timestamp = DateTime.UtcNow
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conversation ID: {conversationId}", conversationId);
            }
        }

        public async Task SendMessage(MessageCreateDto request)
        {
            try
            {
                var senderIdStr = Context.User?.FindFirst("nameid")?.Value;
                if (string.IsNullOrEmpty(senderIdStr))
                {
                    await Clients.Caller.SendAsync("Error", "Unauthorized");
                    return;
                }
                var senderId = int.Parse(senderIdStr);

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    _logger.LogWarning("Message content cannot be empty");
                    await Clients.Caller.SendAsync("Error", "Message content cannot be empty");
                    return;
                }

                var createdMessage = await _messageService.CreateAsync(request, senderId);
                if (createdMessage == null)
                {
                    await Clients.Caller.SendAsync("Error", "Failed to send message");
                    return;
                }

                _logger.LogInformation("User {userId} sent messages {messages} to conversation {conversationId}",
                    senderId, createdMessage, request.ConversationId);

                await Clients.Group($"conversation_{request.ConversationId}")
                    .SendAsync("ReceiveMessage", createdMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                await Clients.Caller.SendAsync("Error", "Failed to send message");
            }
        } 

        public async Task DeleteMessage(int messageId, int conversationId)
        {
            try
            {
                var userIdStr = Context.User?.FindFirst("nameid")?.Value;
                if (string.IsNullOrEmpty(userIdStr))
                {
                    await Clients.Caller.SendAsync("Error", "Unauthorized");
                    return;
                }

                var userId = int.Parse(userIdStr);
                var message = await _messageService.GetByIdAsync(messageId);
                if (message == null)
                {
                    await Clients.Caller.SendAsync("Error", "Message not found");
                    return;
                }

                var deleted = await _messageService.DeleteByIdAsync(message.Id);
                if (!deleted)
                {
                    await Clients.Caller.SendAsync("Error", "Failed to delete message");
                    return;
                }

                _logger.LogInformation("User {userId} deleted message {messageId}", userId, messageId);
                await Clients.Groups($"conversation_{conversationId}")
                    .SendAsync("MessageDeleted", new
                    {
                        messageId,
                        userId,
                        conversationId,
                        timestamp = DateTime.UtcNow
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting messages");
                await Clients.Caller.SendAsync("Error", "Failed to delete message");
            }
        }

        public async Task UpdateMessage(int messageId, string newContent, int conversationId)
        {
            try
            {
                var userIdStr = Context.User?.FindFirst("nameid")?.Value;
                if (string.IsNullOrEmpty(userIdStr))
                {
                    await Clients.Caller.SendAsync("Error", "Unauthorized");
                    return;
                }

                var userId = int.Parse(userIdStr);
                if (string.IsNullOrEmpty(newContent))
                {
                    await Clients.Caller.SendAsync("Error", "Message content cannot be empty");
                    return;
                }

                var message = await _messageService.GetByIdAsync(userId);
                if (message == null)
                {
                    await Clients.Caller.SendAsync("Error", "Message not found");
                    return;
                }

                var updatedMessage = await _messageService.UpdateByIdAsync(message.Id,
                    new MessageUpdateDto { Content = newContent });
                if (updatedMessage == null)
                {
                    await Clients.Caller.SendAsync("Error", "Failed to update message");
                    return;
                }

                _logger.LogInformation("User {userId} edited message {messageId}", userId, messageId);

                await Clients.Groups($"conversation_{conversationId}")
                    .SendAsync("MessageUpdated", updatedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing messages");
                await Clients.Caller.SendAsync("Error", "Failed to delete message");
            }
        }

    }
}
