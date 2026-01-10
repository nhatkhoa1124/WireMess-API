using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WireMess.Models.DTOs.Request.Conversation;
using WireMess.Models.DTOs.Response.Conversation;
using WireMess.Services.Interfaces;

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
            catch(Exception ex)
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ConversationDto>> GetByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationService.GetByIdAsync(id);
                if(conversation == null)
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
    }
}
