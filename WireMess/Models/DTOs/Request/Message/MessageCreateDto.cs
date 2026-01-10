using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.Message
{
    public class MessageCreateDto
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int ConversationId { get; set; }
        public ICollection<IFormFile>? Attachments { get; set; }
    }
}
