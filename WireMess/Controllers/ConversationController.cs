using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WireMess.Models.DTOs.Request.Conversation;
using WireMess.Models.DTOs.Response.Conversation;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(IConversationService conversationService,
            ILogger<ConversationController> logger)
        {
            _conversationService = conversationService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ConversationDto>> CreateAsync(ConversationCreateUpdateDto request)
        {
            try
            {
                var createdConversation = await _conversationService.CreateAsync(request);
                if (createdConversation == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return Created($"/api/conversations/{createdConversation.Id}", createdConversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new conversation");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetAllAsync()
        {
            try
            {
                var conversations = await _conversationService.GetAllAsync();
                if (conversations == null)
                    return NoContent();
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all conversations");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetMyConversations()
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                    return Unauthorized("User not authorized");
                var conversations = await _conversationService.GetAllByUserIdAsync(userId.Value);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversations for user ID: {id}", User.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ConversationDto>> GetByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationService.GetByIdAsync(id);
                if (conversation == null)
                    return NotFound("Conversation not found");
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation ID: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ConversationDto>> UpdateByIdAsync(int id, ConversationCreateUpdateDto request)
        {
            try
            {
                var updatedConversation = await _conversationService.UpdateGroupProfileByIdAsync(id, request);
                if (updatedConversation == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                return Ok(updatedConversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating conversation ID: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var deleted = await _conversationService.DeleteByIdAsync(id);
                if (!deleted)
                    return NotFound($"Conversation ID: {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting conversation ID: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost("{id}/members/{userId}")]
        public async Task<IActionResult> AddMemberToConversationAsync(int userId, int id)
        {
            try
            {
                var success = await _conversationService.AddUserToConversationByIdAsync(userId, id);
                if (!success)
                {
                    _logger.LogWarning("Error adding member to conversation");
                    return BadRequest("Failed to add member to conversation");
                }
                return Ok("Member added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding member ID: {userId} to conversation ID: {conversationId}",
                    userId, id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMemberToConversationAsync(int id, int userId)
        {
            try
            {
                var success = await _conversationService.RemoveUserFromConversationByIdAsync(userId, id);
                if (!success)
                {
                    _logger.LogWarning("Error removing member from conversation");
                    return NotFound("Failed to remove member from conversation");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error removing member ID: {userId} from conversation ID: {conversationId}",
                    userId, id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
