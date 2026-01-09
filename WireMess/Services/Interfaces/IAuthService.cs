using Microsoft.AspNetCore.Authentication;
using WireMess.Models.DTOs.Request.User;
using WireMess.Models.DTOs.Response.User;

namespace WireMess.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto request);
        Task<AuthResultDto> LoginAsync(LoginDto request);
        Task<bool> LogoutAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request);
        Task<bool> ValidateTokenAsync(string token);
    }
}
