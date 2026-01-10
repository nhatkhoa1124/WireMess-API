using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.Conversation
{
    public class ConversationCreateUpdateDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string ConversationName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
