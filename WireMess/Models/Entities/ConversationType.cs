namespace WireMess.Models.Entities
{
    public class ConversationType : BaseEntity
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }

        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
