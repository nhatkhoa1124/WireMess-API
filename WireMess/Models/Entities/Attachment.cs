namespace WireMess.Models.Entities
{
    public class Attachment : BaseEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public int MessageId { get; set; }
        public string StoragePath { get; set; }
        public string? PublicId { get; set; }
        public string? FileType { get; set; }

        public Message? Message { get; set; }
    }
}
