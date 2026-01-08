using System.Security.Claims;
using WireMess.Models.Entities;

namespace WireMess.Utils.AuthUtil.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        int? GetUserIdFromToken(string token);
    }
}
