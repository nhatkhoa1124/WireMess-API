using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WireMess.Models.DTOs.Request.Message;
using WireMess.Models.DTOs.Response.Message;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<MessageDto>>> CreateAsync([FromForm] MessageCreateDto request)
        {
            try
            {
                if (!request.IsValid())
                    return BadRequest("Messag must contain either text or attachment");

                var userId = User.GetUserId();
                if (userId == null)
                    return Unauthorized("User not authorized");
                var messages = await _messageService.CreateAsync(request, userId.Value);
                if (messages == null || !messages.Any())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var location = Url.Action(nameof(GetByIdAsync), new { id = messages.First().Id });
                return Created(location, messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("conversation/{conversationId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetByConversationIdAsync(
            int conversationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var messages = await _messageService.GetByConversationIdAsync(conversationId, page, pageSize);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages in conversation ID : {conversationId}", conversationId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetByIdAsync(int id)
        {
            try
            {
                var message = await _messageService.GetByIdAsync(id);
                if (message == null)
                    return NotFound();
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages with ID : {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult<MessageDto>> UpdateByIdAsync(int id, MessageUpdateDto request)
        {
            try
            {
                var message = await _messageService.GetByIdAsync(id);
                if (message == null)
                    return NotFound($"Message ID: {id} not found");
                var updatedMessage = await _messageService.UpdateByIdAsync(id, request);
                if (updatedMessage == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                return Ok(updatedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating message with ID : {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                var success = await _messageService.DeleteByIdAsync(id);
                if (!success)
                    return NotFound($"Message ID: {id} not found");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message with ID : {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
