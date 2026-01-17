using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.User
{
    public class UserProfileRequestDto
    {
        private string? _username;
        private string? _email;
        private string? _phoneNumber;
        private string? _avatarUrl;

        [MinLength(3), MaxLength(50)]
        public string? Username 
        { 
            get => _username; 
            set => _username = string.IsNullOrWhiteSpace(value)?null:value; 
        }
        [EmailAddress]
        public string? Email
        {
            get => _email;
            set => _email = string.IsNullOrWhiteSpace(value) ? null : value;
        }
        [Phone]
        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = string.IsNullOrWhiteSpace(value) ? null : value;
        }
        public IFormFile? Avatar { get; set; }
    }
}
