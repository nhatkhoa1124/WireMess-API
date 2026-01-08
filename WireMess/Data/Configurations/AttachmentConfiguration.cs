using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ConfigureBaseEntity();

            builder.ToTable("Attachments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FileName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(a => a.FileSize)
                .IsRequired();
            builder.Property(a => a.StoragePath)
                .HasMaxLength(500)
                .IsRequired();
        }
    }
}
