namespace WireMess.Models.DTOs.Response.User
{
    public class UserPublicDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActive { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
