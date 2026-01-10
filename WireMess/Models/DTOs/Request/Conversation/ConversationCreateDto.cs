using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.Conversation
{
    public class ConversationCreateUpdateDto
    {
        [StringLength(50)]
        public string ConversationName { get; set; }
        [StringLength(200)]
        public string AvatarUrl { get; set; }
    }
}
