using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response.User;
using WireMess.Models.Entities;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;
using WireMess.Utils.AuthUtil.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request)
        {
            try
            {
                if(request.NewPassword != request.ConfirmPassword)
                {
                    _logger.LogWarning("New password and password confirm are different");
                    return false;
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    _logger.LogWarning("Change password failed: User {UserId} not found", userId);
                    return false;
                }
                var isCurrentPasswordValid = _passwordHasher.VerifyHash(
                    request.CurrentPassword, 
                    user.PasswordHash,
                    user.PasswordSalt);

                if(!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Change password failed: Current password incorrect for ID: {UserId}", userId);
                    return false;
                }

                var newPassword = _passwordHasher.CreateHash(request.NewPassword);
                user.PasswordHash = newPassword.Hash;
                user.PasswordSalt = newPassword.Salt;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto request)
        {
            try
            {
                User user = null;
                if(!string.IsNullOrWhiteSpace(request.Username))
                {
                    user = await _userRepository.GetByUsernameAsync(request.Username.Trim().ToLower());
                    if(user == null)
                    {
                        _logger.LogWarning("Login failed: User not found username {username}", request.Username);
                        return AuthResultDto.Failure("Invalid username");
                    }
                }
                else
                {
                    _logger.LogWarning("Login failed: No username provided");
                    return AuthResultDto.Failure("Invalid username");
                }

                    var isPasswordValid = _passwordHasher.VerifyHash(request.Password, user.PasswordHash, user.PasswordSalt);
                if(!isPasswordValid)
                {
                    _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                    return AuthResultDto.Failure("Invalid password");
                }

                user.IsOnline = true;
                user.LastActive = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Login successful for user {UserId}", user.Id);
                return GenerateAuthenticationResult(user);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error logging in for user with credential: {UsernameOrEmail}", 
                    request.Username);
                return AuthResultDto.Failure($"Login failed: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    _logger.LogWarning("Logout failed: User {UserId} not found", userId);
                    return false;
                }
                user.IsOnline = false;
                user.LastActive = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error loggin out");
                return false;
            }
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto request)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Registration failed: Email or password is empty");
                    return AuthResultDto.Failure("Email and password required");
                }

                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if(existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User with email {Email} already exists", request.Email);
                    return AuthResultDto.Failure("User with this email already exists");
                }

                if(!string.IsNullOrWhiteSpace(request.Username))
                {
                    var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
                    if(existingUsername != null)
                    {
                        _logger.LogWarning("Registration failed: User with this username {Username} already exists",
                            request.Username);
                        return AuthResultDto.Failure("User already taken");
                    }

                }
                var hashedPassword = _passwordHasher.CreateHash(request.Password);

                var user = new User
                {
                    Email = request.Email.Trim().ToLower(),
                    Username = request.Username.Trim(),
                    PasswordHash = hashedPassword.Hash,
                    PasswordSalt = hashedPassword.Salt,
                    PhoneNumber = request?.PhoneNumber,
                    IsOnline = false,
                    LastActive = DateTime.UtcNow,
                    AvatarUrl = ""
                };

                var createdUser = await _userRepository.CreateAsync(user);
                if (createdUser == null)
                {
                    _logger.LogError("Failed to create user in database");
                    return AuthResultDto.Failure("Failed to create user");
                }
                _logger.LogInformation("User registered successfully with ID: {userId}", createdUser.Id);

                return GenerateAuthenticationResult(createdUser);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration. Email: {Email}", request.Email);
                return AuthResultDto.Failure($"Registration failed: {ex.Message}");
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Token validation failed: Token is empty");
                    return Task.FromResult(false);
                }
                var isValid = _jwtService.ValidateToken(token);
                if(!isValid)
                {
                    _logger.LogWarning("Token validation failed: Invalid token");
                }

                return Task.FromResult(isValid);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return Task.FromResult(false);
            }
        }

        // Private helpers
        private AuthResultDto GenerateAuthenticationResult(User user)
        {
            try
            {
                var accessToken = _jwtService.GenerateToken(user);
                var tokenExpiry = GetTokenExpiry(accessToken);
                string refreshToken = null;
                var userDto = user.MapUserToDto();
                return AuthResultDto.SuccessResult(
                    token:accessToken,
                    refreshToken: refreshToken,
                    expiry: tokenExpiry,
                    user: userDto);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error generating authentication result for ID: {userId}",user.Id);
                return AuthResultDto.Failure("Authentication failed");
            }
        }

        private DateTime GetTokenExpiry(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo;
            }
            catch
            {
                return DateTime.UtcNow.AddHours(1);
            }
        }
    }
}
