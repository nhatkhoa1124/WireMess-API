using System.ComponentModel.DataAnnotations;

namespace WireMess.Models.DTOs.Request.Message
{
    public class MessageCreateDto
    {
        public string? Content { get; set; }
        [Required]
        public int ConversationId { get; set; }
        public IFormFile? Attachment { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Content) || Attachment != null;
        }
    }
}
