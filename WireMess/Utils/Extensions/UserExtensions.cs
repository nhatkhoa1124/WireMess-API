using WireMess.Models.DTOs.Response.User;
using WireMess.Models.Entities;

namespace WireMess.Utils.Extensions
{
    public static class UserExtensions
    {
        public static UserDto MapUserToDto(this User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsOnline = user.IsOnline,
                LastActive = user.LastActive,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
