using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.Message
{
    public class MessageUpdateDto
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int ConversationId { get; set; }
    }
}
