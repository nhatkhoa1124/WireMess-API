using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response;
using WireMess.Models.DTOs.Response.User;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                if(!result.Success)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An error occurred during registration",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if(!result.Success)
                {
                    _logger.LogWarning("Login failed for: {Identifier}", request.Username ?? "Unknown");
                    return Unauthorized(new ErrorResponseDto
                    {
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }
                _logger.LogInformation("Login successful");
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An error occurred during login",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.GetUserId();
                if(userId == null)
                {
                    return Unauthorized(new ErrorResponseDto
                    {
                        Message = "User not authenticated"
                    });
                }

                var user = await _authService.GetCurrentUserAsync(userId.Value);
                if(user == null)
                {
                    return NotFound(new ErrorResponseDto
                    {
                        Message = "User not found"
                    });
                }

                return Ok(user);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An error occurred while fetching user profile",
                    Errors = new List<string> { ex.Message}
                });
            }
        }
    }
}
