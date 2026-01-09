namespace WireMess.Models.DTOs.Response.User
{
    public class UserActiveStatusDto
    {
        public int Id { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActive { get; set; }
    }
}
