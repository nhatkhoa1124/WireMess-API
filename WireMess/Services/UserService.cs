using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response.User;
using WireMess.Repositories.Interfaces;
using WireMess.Services.Interfaces;

namespace WireMess.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserActiveStatusDto> GetActiveStatusByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException($"User {userId} not found");

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

                return users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsOnline = u.IsOnline,
                    LastActive = u.LastActive,
                    AvatarUrl = u.AvatarUrl
                }).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
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

                user.Username = request.Username ?? user.Username;
                user.Email = request.Email ?? user.Email;
                user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
                user.AvatarUrl = request.AvatarUrl ?? user.AvatarUrl;
                user.UpdatedAt = DateTime.UtcNow;


                var updatedUser = await _userRepository.UpdateAsync(user);
                if (updatedUser == null)
                    throw new InvalidOperationException($"Error updating user ID: {userId}");
                return new UserProfileResponseDto
                {
                    Id = user.Id,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    PhoneNumber = updatedUser.PhoneNumber,
                    AvatarUrl = updatedUser.AvatarUrl
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
