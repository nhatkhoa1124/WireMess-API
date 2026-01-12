using WireMess.Models.Entities;

namespace WireMess.Models.DTOs.Response.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
