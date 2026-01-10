using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response;
using WireMess.Models.DTOs.Response.User;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.GetUserId();
                if(userId == null)
                {
                    return Unauthorized("User not found");
                }
                await _authService.LogoutAsync(userId.Value);
                return Ok(new {Message = "Logged out successfully"});
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during logging out");
                return Ok(new {Message = "Logged out"});
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                var userId = User.GetUserId();
                _logger.LogInformation($"{ userId.Value} ");
                if(userId == null)
                {
                    return Unauthorized("User not found");
                }
                bool success = await _authService.ChangePasswordAsync(userId.Value, request);
                if(!success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponseDto
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "An unexpected error occurred"
                    });
                }

                return Ok(new {Message = "Password changed successfully"});
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", User.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ErrorResponseDto
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "An unexpected error occurred",
                        Errors = new List<string>{ ex.Message }
                    });

            }
        }
    }
}
