namespace WireMess.Models.DTOs.Response.Message
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; }
        public string? PublicId { get; set; }
        public string ? FileType { get; set; }
    }
}
