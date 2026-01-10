using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response;
using WireMess.Models.DTOs.Response.User;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPublicDto>>> GetAllUsersPublic()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                var publicUsers = users.Select(u => new UserPublicDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    IsOnline = u.IsOnline,
                    LastActive = u.LastActive,
                    AvatarUrl = u.AvatarUrl
                }).ToList();
                return Ok(publicUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _userService.GetCurrentUserAsync(userId.Value);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An error occurred while fetching user profile",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user ID: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet("active-status/{id}")]
        public async Task<ActionResult<UserActiveStatusDto>> GetUserActiveStatusById(int id)
        {
            try
            {
                var userStatus = await _userService.GetActiveStatusByIdAsync(id);
                if (userStatus == null)
                    return NotFound("User status not found");
                return Ok(userStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user active status for ID: {userId}", User.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("me/update")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateCurrentUserProfile(UserProfileRequestDto request)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                    return Unauthorized("User not authorized");
                var updatedUser = await _userService.UpdateProfileByIdAsync(userId.Value, request);
                if (updatedUser == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile ID: {userId}", User.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateUserProfileById(int id, UserProfileRequestDto request)
        {
            try
            {
                var updatedUser = await _userService.UpdateProfileByIdAsync(id, request);
                if (updatedUser == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile ID: {userId}", User.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
