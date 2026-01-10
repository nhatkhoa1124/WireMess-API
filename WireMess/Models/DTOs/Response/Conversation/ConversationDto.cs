namespace WireMess.Models.DTOs.Response.Conversation
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string ConversationName { get; set; }
        public DateTime LastMessageAt { get; set; }
        public string AvatarUrl { get; set; }
    }
}
