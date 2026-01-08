namespace WireMess.Models.DTOs.Response.User
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public UserDto? User { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static AuthResultDto SuccessResult(
            string token,
            string refreshToken,
            DateTime expiry,
            UserDto user,
            string message = "Authentication successful")
        {
            return new AuthResultDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = expiry,
                User = user
            };
        }

        public static AuthResultDto Failure(string message, params string[] errors)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = message,
                Errors = errors.ToList()
            };
        }
    }
}
