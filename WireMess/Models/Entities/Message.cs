namespace WireMess.Models.Entities
{
    public class Message : BaseEntity
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public Attachment? Attachment { get; set; }

        public User? Sender { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
