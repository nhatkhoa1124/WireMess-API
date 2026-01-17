using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using WireMess.Models.Entities;

namespace WireMess.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ConfigureBaseEntity();

            builder.ToTable("Messages");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.HasOne(m => m.Attachment)
                .WithOne(a => a.Message)
                .HasForeignKey<Attachment>(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
