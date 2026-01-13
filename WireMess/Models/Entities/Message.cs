namespace WireMess.Models.Entities
{
    public class Message : BaseEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }

        public User? Sender { get; set; }
        public Conversation? Conversation { get; set; }
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
