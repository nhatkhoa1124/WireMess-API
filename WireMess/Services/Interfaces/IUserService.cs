using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response.User;

namespace WireMess.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserProfileResponseDto> UpdateProfileByIdAsync(int userId, UserProfileRequestDto request);
        Task<UserActiveStatusDto> GetActiveStatusByIdAsync(int userId);
    }
}
