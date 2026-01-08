using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.User
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
