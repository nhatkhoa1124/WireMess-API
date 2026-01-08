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
                .IsRequired();

            builder.HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
