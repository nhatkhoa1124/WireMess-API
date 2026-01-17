using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response.User;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;
using WireMess.Utils.Extensions;

namespace WireMess.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ICloudinaryService cloudinaryService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return null;
                }
                return user.MapUserToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserActiveStatusDto> GetActiveStatusByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception($"User {userId} not found");

                return new UserActiveStatusDto
                {
                    Id = user.Id,
                    IsOnline = user.IsOnline,
                    LastActive = user.LastActive
                }; 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting active status for user ID: {userId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null)
                    return Enumerable.Empty<UserDto>();

                return users.Select(u => u.MapUserToDto()).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserDto> GetByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException("User not found");
                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsOnline = user.IsOnline,
                    LastActive = user.LastActive,
                    AvatarUrl = user.AvatarUrl
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {userId}", userId);
                throw;
            }
        }

        public async Task<UserProfileResponseDto> UpdateProfileByIdAsync(int userId, UserProfileRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException($"User {userId} not found");

                if (!string.IsNullOrWhiteSpace(request.Username))
                    user.Username = request.Username;
                if (!string.IsNullOrWhiteSpace(request.Email))
                    user.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                    user.PhoneNumber = request.PhoneNumber;

                if (request.Avatar != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(user.AvatarPublicId))
                        {
                            await _cloudinaryService.DeleteAvatarAsync(user.AvatarPublicId);
                        }

                        var uploadResult = await _cloudinaryService.UploadAvatarAsync(
                            request.Avatar,
                            userId.ToString());

                        user.AvatarUrl = uploadResult.SecureUrl.ToString();
                        user.AvatarPublicId = uploadResult.PublicId;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload avatar for user ID: {userId}", userId);
                        throw new Exception("Failed to upload avatar", ex);
                    }
                }
                user.UpdatedAt = DateTime.UtcNow;
                var updatedUser = await _userRepository.UpdateAsync(user);
                if (updatedUser == null)
                    throw new InvalidOperationException($"Error updating user ID: {userId}");

                return new UserProfileResponseDto
                {
                    Id = updatedUser.Id,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    PhoneNumber = updatedUser.PhoneNumber,
                    AvatarUrl = updatedUser.AvatarUrl,
                    IsOnline = updatedUser.IsOnline,
                    LastActive  = updatedUser.LastActive
                };

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating profile ID: {userId}", userId);
                throw;
            }
        }
    }
}
