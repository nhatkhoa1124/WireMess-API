namespace WireMess.Models.DTOs.Response.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActive { get; set; }
        public string AvatarUrl { get; set; }

    }
}
