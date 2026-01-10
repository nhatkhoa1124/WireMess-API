namespace WireMess.Models.Entities
{
    public class Conversation : BaseEntity
    {
        public int Id { get; set; }
        public string? ConversationName { get; set; }
        public int TypeId { get; set; }
        public DateTime LastMessageAt { get; set; }
        public string? AvatarUrl { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<UserConversation> UserConversation { get; set; } = new List<UserConversation>();
        public ConversationType? ConversationType { get; set; }
    }
}
