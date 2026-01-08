namespace WireMess.Models.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActive { get; set; }
        public string AvatarUrl { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<UserConversation> UserConversations { get; set; } = new List<UserConversation>();
    }
}
